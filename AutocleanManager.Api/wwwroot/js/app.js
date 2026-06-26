// Estado em memória.
let sessao;
let usuarios = [];
let veiculos = [];
let tipos = [];
let agendamentos = [];
let filtroVeiculo = "";

const multiplicadorSujeira = { Leve: 1.0, Media: 1.10, Pesada: 1.20 };
const STATUS = ["Aguardando", "Confirmado", "Na fila", "Lavando", "Pronto para retirada", "Finalizado", "Cancelado"];

document.addEventListener("DOMContentLoaded", iniciar);

async function iniciar() {
    sessao = exigirLogin();
    if (!sessao) return;

    configurarPerfil();
    configurarMenu();
    configurarBotoesLimpar();
    configurarFormularios();
    configurarPrevias();
    configurarBuscaVeiculo();

    document.getElementById("sair").addEventListener("click", () => {
        limparSessao();
        window.location.href = "login.html";
    });

    await carregarTudo();
}

/* ---------- Perfil e navegação ---------- */
function configurarPerfil() {
    document.getElementById("usuario-nome").textContent = sessao.nome;
    document.getElementById("usuario-papel").textContent = sessao.papel;
    const saudacao = document.getElementById("saudacao-nome");
    if (saudacao) saudacao.textContent = sessao.nome.split(" ")[0];

    // Remove do DOM as telas e itens de menu que o perfil não pode acessar.
    document.querySelectorAll("[data-perfil]").forEach(el => {
        if (!perfilPermite(el.dataset.perfil)) el.remove();
    });

    // Só o Administrador escolhe o papel ao cadastrar usuário.
    if (!ehAdmin(sessao)) {
        const campoPapel = document.getElementById("campo-papel");
        if (campoPapel) campoPapel.classList.add("escondido");
    }
}

function perfilPermite(perfil) {
    if (perfil === "cliente") return ehCliente(sessao);
    if (perfil === "equipe") return ehEquipe(sessao);
    if (perfil === "admin") return ehAdmin(sessao);
    return true;
}

function configurarMenu() {
    const links = [...document.querySelectorAll(".link-menu")];
    links.forEach(b => b.addEventListener("click", () => ativarSecao(b.dataset.secao)));
    if (links[0]) ativarSecao(links[0].dataset.secao);
}

function ativarSecao(id) {
    document.querySelectorAll(".link-menu").forEach(b => b.classList.toggle("ativo", b.dataset.secao === id));
    document.querySelectorAll(".secao").forEach(s => s.classList.toggle("ativa", s.id === id));
}

/* ---------- Carregamento ---------- */
async function carregarTudo() {
    try {
        [usuarios, veiculos, tipos, agendamentos] = await Promise.all([
            api.usuarios.listar(),
            api.veiculos.listar(),
            api.tipos.listar(),
            api.agendamentos.listar()
        ]);
    } catch (e) {
        avisar(e.message, true);
        return;
    }

    if (ehCliente(sessao)) {
        preencherAgendar();
        renderInicio();
        renderMeusAgendamentos();
    } else {
        preencherSelectsAgendamento();
        preencherSelectVeiculo();
        renderAgenda();
        renderAgendamentos();
        renderVeiculos();
        renderUsuarios();
        renderTipos();
        if (ehAdmin(sessao)) renderDashboard();
    }
}

function veiculosDoCliente() {
    return veiculos.filter(v => v.usuarioId === sessao.id);
}

function meusAgendamentos() {
    return agendamentos.filter(a => a.usuarioId === sessao.id);
}

/* ====================================================================
   CLIENTE
   ==================================================================== */
function renderInicio() {
    const meus = meusAgendamentos();
    const futuros = meus.filter(a => new Date(a.dataHoraAgendada) >= new Date() && a.status !== "Cancelado");

    const cartoes = [
        { rotulo: "Agendamentos em aberto", valor: futuros.length },
        { rotulo: "Total de agendamentos", valor: meus.length },
        { rotulo: "Meus veículos", valor: veiculosDoCliente().length }
    ];
    document.getElementById("resumo-cliente").innerHTML = cartoes.map(cartao).join("");

    const lista = [...futuros].sort((a, b) => new Date(a.dataHoraAgendada) - new Date(b.dataHoraAgendada));
    const corpo = document.querySelector("#tabela-proximos tbody");
    if (lista.length === 0) {
        corpo.innerHTML = `<tr><td class="vazio" colspan="5">Nenhum agendamento em aberto.</td></tr>`;
        return;
    }
    corpo.innerHTML = lista.map(a => `
        <tr>
            <td>${dataHora(a.dataHoraAgendada)}</td>
            <td>${descricaoVeiculo(a.veiculoId)}</td>
            <td>${nomeTipo(a.tipoLavagemId)}</td>
            <td>${moeda(a.precoTotal)}</td>
            <td>${etiquetaStatus(a.status)}</td>
        </tr>
    `).join("");
}

function preencherAgendar() {
    const form = document.getElementById("form-agendar");
    const meus = veiculosDoCliente();
    const aviso = document.getElementById("aviso-sem-veiculo");
    const botao = form.querySelector("button[type=submit]");

    if (meus.length === 0) {
        aviso.classList.remove("escondido");
        form.veiculoId.innerHTML = `<option value="">Nenhum veículo</option>`;
        botao.disabled = true;
    } else {
        aviso.classList.add("escondido");
        botao.disabled = false;
        form.veiculoId.innerHTML = meus.map(v => `<option value="${v.id}">${v.placa} - ${v.marca} ${v.modelo}</option>`).join("");
    }
    form.tipoLavagemId.innerHTML = tipos.map(t => `<option value="${t.id}">${t.nome} — ${moeda(t.precoBase)}</option>`).join("");
    atualizarPrevia("form-agendar", "previa-cliente");
}

async function salvarAgendar(evento) {
    evento.preventDefault();
    const form = evento.target;
    if (!form.veiculoId.value) {
        avisar("Você não tem veículos cadastrados.", true);
        return;
    }
    const dados = {
        usuarioId: sessao.id,
        veiculoId: parseInt(form.veiculoId.value, 10),
        tipoLavagemId: parseInt(form.tipoLavagemId.value, 10),
        nivelSujeira: form.nivelSujeira.value,
        dataHoraAgendada: new Date(form.dataHora.value).toISOString(),
        status: "Aguardando"
    };
    try {
        await api.agendamentos.criar(dados);
        avisar("Agendamento solicitado! Aguarde a confirmação.");
        form.reset();
        await carregarTudo();
        ativarSecao("meus-agendamentos");
    } catch (e) {
        avisar(e.message, true);
    }
}

function renderMeusAgendamentos() {
    const lista = [...meusAgendamentos()].sort((a, b) => new Date(b.dataHoraAgendada) - new Date(a.dataHoraAgendada));
    const corpo = document.querySelector("#tabela-meus tbody");
    if (lista.length === 0) {
        corpo.innerHTML = `<tr><td class="vazio" colspan="7">Você ainda não tem agendamentos.</td></tr>`;
        return;
    }
    corpo.innerHTML = lista.map(a => {
        const podeCancelar = a.status !== "Finalizado" && a.status !== "Cancelado";
        const acao = podeCancelar
            ? `<button class="botao-mini perigo" onclick="cancelarAgendamento(${a.id})">Cancelar</button>`
            : "";
        return `
        <tr>
            <td>${dataHora(a.dataHoraAgendada)}</td>
            <td>${descricaoVeiculo(a.veiculoId)}</td>
            <td>${nomeTipo(a.tipoLavagemId)}</td>
            <td>${a.nivelSujeira}</td>
            <td>${moeda(a.precoTotal)}</td>
            <td>${etiquetaStatus(a.status)}</td>
            <td>${acao}</td>
        </tr>`;
    }).join("");
}

async function cancelarAgendamento(id) {
    if (!confirm("Cancelar este agendamento?")) return;
    const a = agendamentos.find(x => x.id === id);
    try {
        await mudarStatus(a, "Cancelado");
        avisar("Agendamento cancelado.");
        await carregarTudo();
    } catch (e) {
        avisar(e.message, true);
    }
}

/* ====================================================================
   EQUIPE — Agenda do dia
   ==================================================================== */
function renderAgenda() {
    const hoje = new Date().toDateString();
    const lista = agendamentos
        .filter(a => new Date(a.dataHoraAgendada).toDateString() === hoje)
        .sort((a, b) => new Date(a.dataHoraAgendada) - new Date(b.dataHoraAgendada));

    const corpo = document.querySelector("#tabela-agenda tbody");
    if (lista.length === 0) {
        corpo.innerHTML = `<tr><td class="vazio" colspan="5">Nenhuma lavagem agendada para hoje.</td></tr>`;
        return;
    }
    corpo.innerHTML = lista.map(a => `
        <tr>
            <td>${apenasHora(a.dataHoraAgendada)}</td>
            <td>${nomeUsuario(a.usuarioId)}</td>
            <td>${descricaoVeiculo(a.veiculoId)}</td>
            <td>${nomeTipo(a.tipoLavagemId)}</td>
            <td>${seletorStatus(a)}</td>
        </tr>
    `).join("");
}

function seletorStatus(a) {
    const opcoes = STATUS.map(s => `<option ${s === a.status ? "selected" : ""}>${s}</option>`).join("");
    return `<select class="select-status" onchange="aoMudarStatus(${a.id}, this.value)">${opcoes}</select>`;
}

async function aoMudarStatus(id, novoStatus) {
    const a = agendamentos.find(x => x.id === id);
    try {
        await mudarStatus(a, novoStatus);
        avisar("Status atualizado.");
        await carregarTudo();
    } catch (e) {
        avisar(e.message, true);
    }
}

// Reenvia o agendamento mudando apenas o status (a API exige todos os campos).
function mudarStatus(a, novoStatus) {
    return api.agendamentos.atualizar(a.id, {
        usuarioId: a.usuarioId,
        veiculoId: a.veiculoId,
        tipoLavagemId: a.tipoLavagemId,
        nivelSujeira: a.nivelSujeira,
        dataHoraAgendada: a.dataHoraAgendada,
        status: novoStatus
    });
}

/* ====================================================================
   EQUIPE — Agendamentos (CRUD completo)
   ==================================================================== */
function renderAgendamentos() {
    const corpo = document.querySelector("#tabela-agendamentos tbody");
    if (agendamentos.length === 0) {
        corpo.innerHTML = `<tr><td class="vazio" colspan="8">Nenhum agendamento.</td></tr>`;
        return;
    }
    const lista = [...agendamentos].sort((a, b) => new Date(b.dataHoraAgendada) - new Date(a.dataHoraAgendada));
    corpo.innerHTML = lista.map(a => `
        <tr>
            <td>${dataHora(a.dataHoraAgendada)}</td>
            <td>${nomeUsuario(a.usuarioId)}</td>
            <td>${descricaoVeiculo(a.veiculoId)}</td>
            <td>${nomeTipo(a.tipoLavagemId)}</td>
            <td>${a.nivelSujeira}</td>
            <td>${moeda(a.precoTotal)}</td>
            <td>${etiquetaStatus(a.status)}</td>
            <td>
                <div class="acoes-linha">
                    <button class="botao-mini" onclick="editarAgendamento(${a.id})">Editar</button>
                    <button class="botao-mini perigo" onclick="removerAgendamento(${a.id})">Excluir</button>
                </div>
            </td>
        </tr>
    `).join("");
}

function preencherSelectsAgendamento() {
    const form = document.getElementById("form-agendamento");
    form.usuarioId.innerHTML = usuarios.map(u => `<option value="${u.id}">${u.nome}</option>`).join("");
    form.tipoLavagemId.innerHTML = tipos.map(t => `<option value="${t.id}">${t.nome} — ${moeda(t.precoBase)}</option>`).join("");
    atualizarVeiculosDoUsuario();
}

function atualizarVeiculosDoUsuario() {
    const form = document.getElementById("form-agendamento");
    const usuarioId = parseInt(form.usuarioId.value, 10);
    const doCliente = veiculos.filter(v => v.usuarioId === usuarioId);
    form.veiculoId.innerHTML = doCliente.length
        ? doCliente.map(v => `<option value="${v.id}">${v.placa} - ${v.marca} ${v.modelo}</option>`).join("")
        : `<option value="">Cliente sem veículos</option>`;
}

async function salvarAgendamento(evento) {
    evento.preventDefault();
    const form = evento.target;
    const id = form.id.value;
    if (!form.veiculoId.value) {
        avisar("Selecione um veículo (o cliente não possui veículos).", true);
        return;
    }
    const dados = {
        usuarioId: parseInt(form.usuarioId.value, 10),
        veiculoId: parseInt(form.veiculoId.value, 10),
        tipoLavagemId: parseInt(form.tipoLavagemId.value, 10),
        nivelSujeira: form.nivelSujeira.value,
        dataHoraAgendada: new Date(form.dataHora.value).toISOString(),
        status: form.status.value
    };
    try {
        if (id) {
            await api.agendamentos.atualizar(id, dados);
            avisar("Agendamento atualizado.");
        } else {
            await api.agendamentos.criar(dados);
            avisar("Agendamento criado.");
        }
        limparFormulario("form-agendamento");
        await carregarTudo();
    } catch (e) {
        avisar(e.message, true);
    }
}

function editarAgendamento(id) {
    const a = agendamentos.find(x => x.id === id);
    const form = document.getElementById("form-agendamento");
    form.id.value = a.id;
    form.usuarioId.value = a.usuarioId;
    atualizarVeiculosDoUsuario();
    form.veiculoId.value = a.veiculoId;
    form.tipoLavagemId.value = a.tipoLavagemId;
    form.nivelSujeira.value = a.nivelSujeira;
    form.status.value = a.status;
    form.dataHora.value = paraInputLocal(a.dataHoraAgendada);
    atualizarPrevia("form-agendamento", "previa-preco");
    form.querySelector(".titulo-form").textContent = "Editar agendamento";
}

async function removerAgendamento(id) {
    if (!confirm("Remover este agendamento?")) return;
    try {
        await api.agendamentos.remover(id);
        avisar("Agendamento removido.");
        await carregarTudo();
    } catch (e) {
        avisar(e.message, true);
    }
}

/* ====================================================================
   EQUIPE — Veículos
   ==================================================================== */
function renderVeiculos() {
    const corpo = document.querySelector("#tabela-veiculos tbody");
    const termo = filtroVeiculo.toLowerCase();
    const lista = veiculos.filter(v =>
        v.placa.toLowerCase().includes(termo) || nomeUsuario(v.usuarioId).toLowerCase().includes(termo));

    if (lista.length === 0) {
        corpo.innerHTML = `<tr><td class="vazio" colspan="6">Nenhum veículo encontrado.</td></tr>`;
        return;
    }
    corpo.innerHTML = lista.map(v => `
        <tr>
            <td>${v.placa}</td>
            <td>${v.marca} ${v.modelo}</td>
            <td>${v.cor}</td>
            <td>${v.ano || "-"}</td>
            <td>${nomeUsuario(v.usuarioId)}</td>
            <td>
                <div class="acoes-linha">
                    <button class="botao-mini" onclick="editarVeiculo(${v.id})">Editar</button>
                    <button class="botao-mini perigo" onclick="removerVeiculo(${v.id})">Excluir</button>
                </div>
            </td>
        </tr>
    `).join("");
}

function configurarBuscaVeiculo() {
    const busca = document.getElementById("busca-veiculo");
    if (!busca) return;
    busca.addEventListener("input", () => {
        filtroVeiculo = busca.value;
        renderVeiculos();
    });
}

function preencherSelectVeiculo() {
    const select = document.querySelector("#form-veiculo select[name=usuarioId]");
    select.innerHTML = usuarios.map(u => `<option value="${u.id}">${u.nome}</option>`).join("");
}

async function salvarVeiculo(evento) {
    evento.preventDefault();
    const form = evento.target;
    const id = form.id.value;
    const dados = {
        usuarioId: parseInt(form.usuarioId.value, 10),
        marca: form.marca.value.trim(),
        modelo: form.modelo.value.trim(),
        placa: form.placa.value.trim().toUpperCase(),
        cor: form.cor.value.trim(),
        ano: form.ano.value ? parseInt(form.ano.value, 10) : null
    };
    try {
        if (id) {
            await api.veiculos.atualizar(id, dados);
            avisar("Veículo atualizado.");
        } else {
            await api.veiculos.criar(dados);
            avisar("Veículo cadastrado.");
        }
        limparFormulario("form-veiculo");
        await carregarTudo();
    } catch (e) {
        avisar(e.message, true);
    }
}

function editarVeiculo(id) {
    const v = veiculos.find(x => x.id === id);
    const form = document.getElementById("form-veiculo");
    form.id.value = v.id;
    form.usuarioId.value = v.usuarioId;
    form.marca.value = v.marca;
    form.modelo.value = v.modelo;
    form.placa.value = v.placa;
    form.cor.value = v.cor;
    form.ano.value = v.ano || "";
    form.querySelector(".titulo-form").textContent = "Editar veículo";
}

async function removerVeiculo(id) {
    if (!confirm("Remover este veículo?")) return;
    try {
        await api.veiculos.remover(id);
        avisar("Veículo removido.");
        await carregarTudo();
    } catch (e) {
        avisar(e.message, true);
    }
}

/* ====================================================================
   EQUIPE — Usuários
   ==================================================================== */
function renderUsuarios() {
    const corpo = document.querySelector("#tabela-usuarios tbody");
    if (usuarios.length === 0) {
        corpo.innerHTML = `<tr><td class="vazio" colspan="4">Nenhum usuário cadastrado.</td></tr>`;
        return;
    }
    corpo.innerHTML = usuarios.map(u => `
        <tr>
            <td>${u.nome}</td>
            <td>${u.email}</td>
            <td>${u.papel}</td>
            <td>
                <div class="acoes-linha">
                    <button class="botao-mini" onclick="editarUsuario(${u.id})">Editar</button>
                    <button class="botao-mini perigo" onclick="removerUsuario(${u.id})">Excluir</button>
                </div>
            </td>
        </tr>
    `).join("");
}

async function salvarUsuario(evento) {
    evento.preventDefault();
    const form = evento.target;
    const id = form.id.value;

    const dados = { nome: form.nome.value.trim(), email: form.email.value.trim() };
    if (form.senha.value) dados.senha = form.senha.value;
    if (ehAdmin(sessao)) {
        dados.papel = form.papel.value;
    } else if (!id) {
        dados.papel = "Cliente";
    }

    try {
        if (id) {
            await api.usuarios.atualizar(id, dados);
            avisar("Usuário atualizado.");
        } else {
            await api.usuarios.criar(dados);
            avisar("Usuário cadastrado.");
        }
        limparFormulario("form-usuario");
        await carregarTudo();
    } catch (e) {
        avisar(e.message, true);
    }
}

function editarUsuario(id) {
    const u = usuarios.find(x => x.id === id);
    const form = document.getElementById("form-usuario");
    form.id.value = u.id;
    form.nome.value = u.nome;
    form.email.value = u.email;
    form.senha.value = "";
    form.senha.placeholder = "deixe em branco para manter";
    if (ehAdmin(sessao)) form.papel.value = u.papel;
    form.querySelector(".titulo-form").textContent = "Editar usuário";
}

async function removerUsuario(id) {
    if (!confirm("Remover este usuário?")) return;
    try {
        await api.usuarios.remover(id);
        avisar("Usuário removido.");
        await carregarTudo();
    } catch (e) {
        avisar(e.message, true);
    }
}

/* ====================================================================
   EQUIPE — Tipos de Lavagem
   ==================================================================== */
function renderTipos() {
    const corpo = document.querySelector("#tabela-tipos tbody");
    if (tipos.length === 0) {
        corpo.innerHTML = `<tr><td class="vazio" colspan="4">Nenhum tipo cadastrado.</td></tr>`;
        return;
    }
    corpo.innerHTML = tipos.map(t => `
        <tr>
            <td>${t.nome}</td>
            <td>${moeda(t.precoBase)}</td>
            <td>${t.duracaoEstimadaMinutos} min</td>
            <td>
                <div class="acoes-linha">
                    <button class="botao-mini" onclick="editarTipo(${t.id})">Editar</button>
                    <button class="botao-mini perigo" onclick="removerTipo(${t.id})">Excluir</button>
                </div>
            </td>
        </tr>
    `).join("");
}

async function salvarTipo(evento) {
    evento.preventDefault();
    const form = evento.target;
    const id = form.id.value;
    const dados = {
        nome: form.nome.value.trim(),
        precoBase: parseFloat(form.precoBase.value),
        duracaoEstimadaMinutos: parseInt(form.duracao.value, 10)
    };
    try {
        if (id) {
            await api.tipos.atualizar(id, dados);
            avisar("Tipo atualizado.");
        } else {
            await api.tipos.criar(dados);
            avisar("Tipo cadastrado.");
        }
        limparFormulario("form-tipo");
        await carregarTudo();
    } catch (e) {
        avisar(e.message, true);
    }
}

function editarTipo(id) {
    const t = tipos.find(x => x.id === id);
    const form = document.getElementById("form-tipo");
    form.id.value = t.id;
    form.nome.value = t.nome;
    form.precoBase.value = t.precoBase;
    form.duracao.value = t.duracaoEstimadaMinutos;
    form.querySelector(".titulo-form").textContent = "Editar tipo";
}

async function removerTipo(id) {
    if (!confirm("Remover este tipo de lavagem?")) return;
    try {
        await api.tipos.remover(id);
        avisar("Tipo removido.");
        await carregarTudo();
    } catch (e) {
        avisar(e.message, true);
    }
}

/* ====================================================================
   ADMIN — Dashboard
   ==================================================================== */
function renderDashboard() {
    const hoje = new Date().toDateString();
    const finalizados = agendamentos.filter(a => a.status === "Finalizado");
    const faturamento = finalizados.reduce((s, a) => s + a.precoTotal, 0);
    const lavagensHoje = agendamentos.filter(a => new Date(a.dataHoraAgendada).toDateString() === hoje).length;
    const ticket = finalizados.length ? faturamento / finalizados.length : 0;

    const metricas = [
        { rotulo: "Lavagens hoje", valor: lavagensHoje },
        { rotulo: "Total de agendamentos", valor: agendamentos.length },
        { rotulo: "Faturamento (finalizados)", valor: moeda(faturamento) },
        { rotulo: "Ticket médio", valor: moeda(ticket) }
    ];
    document.getElementById("metricas").innerHTML = metricas.map(cartao).join("");

    renderContagem("#tabela-por-tipo tbody", contar(agendamentos, a => nomeTipo(a.tipoLavagemId)));
    renderContagem("#tabela-por-status tbody", contar(agendamentos, a => a.status));
}

function contar(lista, chave) {
    const mapa = {};
    lista.forEach(item => {
        const k = chave(item);
        mapa[k] = (mapa[k] || 0) + 1;
    });
    return Object.entries(mapa).sort((a, b) => b[1] - a[1]);
}

function renderContagem(seletor, pares) {
    const corpo = document.querySelector(seletor);
    if (pares.length === 0) {
        corpo.innerHTML = `<tr><td class="vazio" colspan="2">Sem dados.</td></tr>`;
        return;
    }
    corpo.innerHTML = pares.map(([nome, qtd]) => `<tr><td>${nome}</td><td>${qtd}</td></tr>`).join("");
}

/* ====================================================================
   Formulários e prévia de preço
   ==================================================================== */
function configurarFormularios() {
    ligarSubmit("form-agendar", salvarAgendar);
    ligarSubmit("form-agendamento", salvarAgendamento);
    ligarSubmit("form-veiculo", salvarVeiculo);
    ligarSubmit("form-usuario", salvarUsuario);
    ligarSubmit("form-tipo", salvarTipo);
}

function ligarSubmit(idForm, handler) {
    const form = document.getElementById(idForm);
    if (form) form.addEventListener("submit", handler);
}

function configurarBotoesLimpar() {
    document.querySelectorAll("[data-limpar]").forEach(botao => {
        botao.addEventListener("click", () => limparFormulario(botao.dataset.limpar));
    });
}

function configurarPrevias() {
    const agendar = document.getElementById("form-agendar");
    if (agendar) {
        ["tipoLavagemId", "nivelSujeira"].forEach(campo =>
            agendar[campo].addEventListener("change", () => atualizarPrevia("form-agendar", "previa-cliente")));
    }
    const agendamento = document.getElementById("form-agendamento");
    if (agendamento) {
        agendamento.usuarioId.addEventListener("change", () => {
            atualizarVeiculosDoUsuario();
            atualizarPrevia("form-agendamento", "previa-preco");
        });
        ["tipoLavagemId", "nivelSujeira"].forEach(campo =>
            agendamento[campo].addEventListener("change", () => atualizarPrevia("form-agendamento", "previa-preco")));
    }
}

// Mesma regra do backend: Leve +0%, Média +10%, Pesada +20%.
function atualizarPrevia(idForm, idAlvo) {
    const form = document.getElementById(idForm);
    const tipo = tipos.find(t => t.id === parseInt(form.tipoLavagemId.value, 10));
    const fator = multiplicadorSujeira[form.nivelSujeira.value] || 1;
    document.getElementById(idAlvo).textContent = moeda(tipo ? tipo.precoBase * fator : 0);
}

function limparFormulario(idForm) {
    const form = document.getElementById(idForm);
    form.reset();
    if (form.id) form.id.value = "";
    const titulos = {
        "form-agendamento": "Novo agendamento",
        "form-veiculo": "Novo veículo",
        "form-usuario": "Novo usuário",
        "form-tipo": "Novo tipo"
    };
    const titulo = form.querySelector(".titulo-form");
    if (titulo) titulo.textContent = titulos[idForm];
    if (idForm === "form-agendamento") {
        atualizarVeiculosDoUsuario();
        atualizarPrevia("form-agendamento", "previa-preco");
    }
    if (idForm === "form-usuario") {
        form.senha.placeholder = "mínimo 4 caracteres";
    }
}

/* ---------- Utilidades ---------- */
function cartao(c) {
    return `<div class="cartao-numero"><span>${c.rotulo}</span><strong>${c.valor}</strong></div>`;
}

function nomeUsuario(id) {
    const u = usuarios.find(x => x.id === id);
    return u ? u.nome : "-";
}

function descricaoVeiculo(id) {
    const v = veiculos.find(x => x.id === id);
    return v ? `${v.placa} (${v.marca} ${v.modelo})` : "-";
}

function nomeTipo(id) {
    const t = tipos.find(x => x.id === id);
    return t ? t.nome : "-";
}

function etiquetaStatus(status) {
    const classes = {
        "Aguardando": "aguardando",
        "Confirmado": "confirmado",
        "Na fila": "fila",
        "Lavando": "lavando",
        "Pronto para retirada": "pronto",
        "Finalizado": "finalizado",
        "Cancelado": "cancelado"
    };
    return `<span class="etiqueta ${classes[status] || ""}">${status}</span>`;
}

function moeda(valor) {
    return (valor || 0).toLocaleString("pt-BR", { style: "currency", currency: "BRL" });
}

function dataHora(iso) {
    return new Date(iso).toLocaleString("pt-BR", { day: "2-digit", month: "2-digit", year: "numeric", hour: "2-digit", minute: "2-digit" });
}

function apenasHora(iso) {
    return new Date(iso).toLocaleTimeString("pt-BR", { hour: "2-digit", minute: "2-digit" });
}

function paraInputLocal(iso) {
    const d = new Date(iso);
    const ajuste = new Date(d.getTime() - d.getTimezoneOffset() * 60000);
    return ajuste.toISOString().slice(0, 16);
}

let timerAviso;
function avisar(mensagem, erro = false) {
    const caixa = document.getElementById("aviso");
    caixa.textContent = mensagem;
    caixa.className = "aviso mostra" + (erro ? " erro" : "");
    clearTimeout(timerAviso);
    timerAviso = setTimeout(() => caixa.classList.remove("mostra"), 3200);
}
