// Estado em memória para evitar várias chamadas e montar os relacionamentos.
let usuarios = [];
let veiculos = [];
let tipos = [];
let agendamentos = [];

const multiplicadorSujeira = { Leve: 1.0, Media: 1.10, Pesada: 1.20 };

document.addEventListener("DOMContentLoaded", iniciar);

function iniciar() {
    configurarMenu();
    configurarBotoesLimpar();
    configurarFormularios();
    configurarPreviaAgendamento();
    carregarTudo();
}

/* ---------- Navegação ---------- */
function configurarMenu() {
    document.querySelectorAll(".link-menu").forEach(botao => {
        botao.addEventListener("click", () => {
            document.querySelectorAll(".link-menu").forEach(b => b.classList.remove("ativo"));
            document.querySelectorAll(".secao").forEach(s => s.classList.remove("ativa"));
            botao.classList.add("ativo");
            document.getElementById(botao.dataset.secao).classList.add("ativa");
        });
    });
}

function configurarBotoesLimpar() {
    document.querySelectorAll("[data-limpar]").forEach(botao => {
        botao.addEventListener("click", () => limparFormulario(botao.dataset.limpar));
    });
}

/* ---------- Carregamento geral ---------- */
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

    renderUsuarios();
    renderTipos();
    renderVeiculos();
    renderAgendamentos();
    preencherSelectsAgendamento();
    preencherSelectVeiculo();
    renderPainel();
}

/* ---------- Painel ---------- */
function renderPainel() {
    const hoje = new Date().toDateString();
    const deHoje = agendamentos.filter(a => new Date(a.dataHoraAgendada).toDateString() === hoje);

    const faturamento = agendamentos
        .filter(a => a.status === "Finalizado")
        .reduce((soma, a) => soma + a.precoTotal, 0);

    const cartoes = [
        { rotulo: "Agendamentos hoje", valor: deHoje.length },
        { rotulo: "Total de agendamentos", valor: agendamentos.length },
        { rotulo: "Veículos cadastrados", valor: veiculos.length },
        { rotulo: "Faturamento (finalizados)", valor: moeda(faturamento) }
    ];

    document.getElementById("cartoes-resumo").innerHTML = cartoes.map(c => `
        <div class="cartao-numero">
            <span>${c.rotulo}</span>
            <strong>${c.valor}</strong>
        </div>
    `).join("");

    const corpo = document.querySelector("#tabela-agenda-hoje tbody");
    const lista = [...deHoje].sort((a, b) => new Date(a.dataHoraAgendada) - new Date(b.dataHoraAgendada));

    if (lista.length === 0) {
        corpo.innerHTML = `<tr><td class="vazio" colspan="5">Nenhum agendamento para hoje.</td></tr>`;
        return;
    }

    corpo.innerHTML = lista.map(a => `
        <tr>
            <td>${apenasHora(a.dataHoraAgendada)}</td>
            <td>${nomeUsuario(a.usuarioId)}</td>
            <td>${descricaoVeiculo(a.veiculoId)}</td>
            <td>${nomeTipo(a.tipoLavagemId)}</td>
            <td>${etiquetaStatus(a.status)}</td>
        </tr>
    `).join("");
}

/* ---------- Usuários ---------- */
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
    const dados = {
        nome: form.nome.value.trim(),
        email: form.email.value.trim(),
        papel: form.papel.value
    };
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
    form.papel.value = u.papel;
    form.querySelector(".titulo-form").textContent = "Editar usuário";
    irPara("usuarios");
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

/* ---------- Tipos de lavagem ---------- */
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
    irPara("tipos");
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

/* ---------- Veículos ---------- */
function renderVeiculos() {
    const corpo = document.querySelector("#tabela-veiculos tbody");
    if (veiculos.length === 0) {
        corpo.innerHTML = `<tr><td class="vazio" colspan="6">Nenhum veículo cadastrado.</td></tr>`;
        return;
    }
    corpo.innerHTML = veiculos.map(v => `
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
    irPara("veiculos");
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

/* ---------- Agendamentos ---------- */
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
    atualizarVeiculosDoCliente();
}

// Mostra apenas os veículos do cliente escolhido no agendamento.
function atualizarVeiculosDoCliente() {
    const form = document.getElementById("form-agendamento");
    const usuarioId = parseInt(form.usuarioId.value, 10);
    const doCliente = veiculos.filter(v => v.usuarioId === usuarioId);
    form.veiculoId.innerHTML = doCliente.length
        ? doCliente.map(v => `<option value="${v.id}">${v.placa} - ${v.marca} ${v.modelo}</option>`).join("")
        : `<option value="">Cliente sem veículos</option>`;
}

function configurarPreviaAgendamento() {
    const form = document.getElementById("form-agendamento");
    form.usuarioId.addEventListener("change", () => {
        atualizarVeiculosDoCliente();
        atualizarPreviaPreco();
    });
    form.tipoLavagemId.addEventListener("change", atualizarPreviaPreco);
    form.nivelSujeira.addEventListener("change", atualizarPreviaPreco);
}

// Mesma regra do backend: Leve +0%, Média +10%, Pesada +20%.
function atualizarPreviaPreco() {
    const form = document.getElementById("form-agendamento");
    const tipo = tipos.find(t => t.id === parseInt(form.tipoLavagemId.value, 10));
    const fator = multiplicadorSujeira[form.nivelSujeira.value] || 1;
    const total = tipo ? tipo.precoBase * fator : 0;
    document.getElementById("previa-preco").textContent = moeda(total);
}

async function salvarAgendamento(evento) {
    evento.preventDefault();
    const form = evento.target;
    const id = form.id.value;

    if (!form.veiculoId.value) {
        avisar("Selecione um veículo (o cliente não possui veículos cadastrados).", true);
        return;
    }

    const dados = {
        usuarioId: parseInt(form.usuarioId.value, 10),
        veiculoId: parseInt(form.veiculoId.value, 10),
        tipoLavagemId: parseInt(form.tipoLavagemId.value, 10),
        nivelSujeira: form.nivelSujeira.value,
        // datetime-local é horário local; a API trabalha em UTC.
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
    atualizarVeiculosDoCliente();
    form.veiculoId.value = a.veiculoId;
    form.tipoLavagemId.value = a.tipoLavagemId;
    form.nivelSujeira.value = a.nivelSujeira;
    form.status.value = a.status;
    form.dataHora.value = paraInputLocal(a.dataHoraAgendada);
    atualizarPreviaPreco();
    form.querySelector(".titulo-form").textContent = "Editar agendamento";
    irPara("agendamentos");
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

/* ---------- Formulários ---------- */
function configurarFormularios() {
    document.getElementById("form-usuario").addEventListener("submit", salvarUsuario);
    document.getElementById("form-tipo").addEventListener("submit", salvarTipo);
    document.getElementById("form-veiculo").addEventListener("submit", salvarVeiculo);
    document.getElementById("form-agendamento").addEventListener("submit", salvarAgendamento);
}

function limparFormulario(idForm) {
    const form = document.getElementById(idForm);
    form.reset();
    form.id.value = "";
    const titulos = {
        "form-usuario": "Novo usuário",
        "form-tipo": "Novo tipo",
        "form-veiculo": "Novo veículo",
        "form-agendamento": "Novo agendamento"
    };
    form.querySelector(".titulo-form").textContent = titulos[idForm];
    if (idForm === "form-agendamento") {
        atualizarVeiculosDoCliente();
        atualizarPreviaPreco();
    }
}

/* ---------- Utilidades ---------- */
function irPara(secao) {
    document.querySelector(`.link-menu[data-secao="${secao}"]`).click();
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

// Converte ISO (UTC) para o formato aceito pelo input datetime-local.
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
