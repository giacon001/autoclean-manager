// Comunicação com a API. A interface é servida pela própria API,
// então usamos caminho relativo "/api".
const API = "/api";

async function requisitar(metodo, rota, corpo) {
    const opcoes = {
        method: metodo,
        headers: { "Content-Type": "application/json" }
    };
    if (corpo !== undefined) {
        opcoes.body = JSON.stringify(corpo);
    }

    const resposta = await fetch(API + rota, opcoes);

    // DELETE retorna 204 sem conteúdo
    if (resposta.status === 204) {
        return null;
    }

    const dados = await resposta.json().catch(() => null);

    if (!resposta.ok) {
        const mensagem = dados && dados.message ? dados.message : "Erro ao comunicar com o servidor.";
        throw new Error(mensagem);
    }

    return dados;
}

const api = {
    usuarios: {
        listar: () => requisitar("GET", "/usuarios"),
        criar: (u) => requisitar("POST", "/usuarios", u),
        atualizar: (id, u) => requisitar("PUT", `/usuarios/${id}`, u),
        remover: (id) => requisitar("DELETE", `/usuarios/${id}`)
    },
    veiculos: {
        listar: () => requisitar("GET", "/veiculos"),
        criar: (v) => requisitar("POST", "/veiculos", v),
        atualizar: (id, v) => requisitar("PUT", `/veiculos/${id}`, v),
        remover: (id) => requisitar("DELETE", `/veiculos/${id}`)
    },
    tipos: {
        listar: () => requisitar("GET", "/tipos-lavagem"),
        criar: (t) => requisitar("POST", "/tipos-lavagem", t),
        atualizar: (id, t) => requisitar("PUT", `/tipos-lavagem/${id}`, t),
        remover: (id) => requisitar("DELETE", `/tipos-lavagem/${id}`)
    },
    agendamentos: {
        listar: () => requisitar("GET", "/agendamentos"),
        criar: (a) => requisitar("POST", "/agendamentos", a),
        atualizar: (id, a) => requisitar("PUT", `/agendamentos/${id}`, a),
        remover: (id) => requisitar("DELETE", `/agendamentos/${id}`)
    }
};
