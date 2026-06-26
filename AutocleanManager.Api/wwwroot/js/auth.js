// Controle de sessão guardado no navegador (localStorage).
// Não é um login com token/segurança real — é o suficiente para o sistema
// reconhecer quem entrou e mostrar as telas certas para cada perfil.
const CHAVE_SESSAO = "autoclean.sessao";

function salvarSessao(usuario) {
    localStorage.setItem(CHAVE_SESSAO, JSON.stringify(usuario));
}

function obterSessao() {
    const dado = localStorage.getItem(CHAVE_SESSAO);
    return dado ? JSON.parse(dado) : null;
}

function limparSessao() {
    localStorage.removeItem(CHAVE_SESSAO);
}

// Usada nas páginas internas: se não há sessão, volta para o login.
function exigirLogin() {
    const sessao = obterSessao();
    if (!sessao) {
        window.location.href = "login.html";
    }
    return sessao;
}

function ehAdmin(s) { return s && s.papel === "Administrador"; }
function ehFuncionario(s) { return s && s.papel === "Funcionario"; }
function ehCliente(s) { return s && s.papel === "Cliente"; }

// Funcionário e Administrador formam a "equipe" (parte operacional).
function ehEquipe(s) { return ehAdmin(s) || ehFuncionario(s); }
