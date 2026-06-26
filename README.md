# AutoClean Manager

Sistema de gerenciamento de agendamentos de um lava-jato. O usuário cadastra
clientes, veículos e tipos de lavagem, e marca lavagens com cálculo automático
de preço e controle de horários.

O projeto tem duas partes que rodam juntas:

- uma **API REST** em ASP.NET Core (C# / .NET 10) com banco PostgreSQL;
- uma **interface web** (HTML, CSS e JavaScript) servida pela própria API.

---

## Como rodar

Pré-requisito: Docker com suporte a Compose.

Na raiz do projeto:

```bash
docker compose up --build -d
```

Isso sobe o banco PostgreSQL e a API. As migrations rodam sozinhas na
inicialização, criando as tabelas. O banco começa **vazio** — tudo é cadastrado
pela interface.

Depois é só abrir no navegador:

| O quê             | Endereço                          |
|-------------------|-----------------------------------|
| Interface web     | http://localhost:8080             |
| Documentação API  | http://localhost:8080/swagger     |

Para parar:

```bash
docker compose down        # para os containers
docker compose down -v     # para e apaga também os dados do banco
```

---

## Primeiro acesso

O banco começa vazio, então é preciso criar a primeira conta:

1. Abra `http://localhost:8080` e clique em **Cadastre-se**.
2. O **primeiro usuário cadastrado vira Administrador** automaticamente, para
   destravar o sistema.
3. Com a conta de administrador você cria os demais usuários (funcionários e
   clientes), veículos e tipos de lavagem.

A partir do segundo cadastro, quem se cadastra pela tela pública entra como
**Cliente**.

---

## Perfis de acesso

Cada perfil enxerga um conjunto de telas diferente:

- **Cliente** — agenda lavagens para os próprios veículos, acompanha o status e
  vê seu histórico. Não cadastra veículos, usuários nem tipos de lavagem.
- **Funcionário** — vê a agenda do dia e atualiza o status das lavagens, e
  cadastra veículos, clientes, tipos de lavagem e agendamentos.
- **Administrador** — tudo que o funcionário faz, mais o **Dashboard** com
  métricas e a gestão completa de usuários (inclusive definir o papel de cada um).

O login é simples (e-mail e senha) e a sessão fica guardada no navegador. Não há
token/segurança avançada — o objetivo é apenas separar o que cada perfil acessa.

---

## A interface

Telas por perfil:

**Cliente**
- **Início** — resumo pessoal e próximos agendamentos.
- **Agendar lavagem** — escolhe veículo, tipo e nível de sujeira; o valor
  estimado já aparece na hora.
- **Meus agendamentos** — histórico, com opção de cancelar os que estão em aberto.

**Funcionário e Administrador**
- **Agenda do dia** — lavagens de hoje, com troca de status direto na lista.
- **Agendamentos / Veículos / Usuários / Tipos de Lavagem** — cadastro completo
  (criar, editar e excluir), com busca de veículos por placa ou cliente.
- **Dashboard** (só Administrador) — faturamento, ticket médio e lavagens por
  tipo e por status.

A interface conversa com a API por `fetch` e, como é servida pela própria API,
não precisa de configuração de CORS nem de outro servidor.

---

## Como o preço é calculado

O preço sai do preço base do tipo de lavagem mais um acréscimo pelo nível de
sujeira:

| Nível de sujeira | Acréscimo |
|------------------|-----------|
| Leve             | +0%       |
| Média            | +10%      |
| Pesada           | +20%      |

Exemplo: Lavagem completa (R$60) com sujeira Pesada → R$72.

A tela mostra esse valor como prévia enquanto você preenche, mas o valor oficial
é sempre o que a API devolve ao salvar.

---

## Regras de negócio

Validações feitas pela API (os avisos aparecem no canto da tela):

- e-mail e placa não podem se repetir;
- um veículo só pode ser agendado pelo seu próprio dono;
- não é possível marcar dois agendamentos ativos no mesmo horário;
- não dá para excluir usuário, veículo ou tipo de lavagem que ainda tenha
  agendamentos vinculados.

Status possíveis de um agendamento:

```
Aguardando → Confirmado → Na fila → Lavando → Pronto para retirada → Finalizado
```

(e `Cancelado` a qualquer momento).

---

## Endpoints da API

Todos seguem o padrão REST com CRUD completo (GET, POST, PUT, DELETE):

| Recurso          | Rota base              |
|------------------|------------------------|
| Usuários         | `/api/usuarios`        |
| Veículos         | `/api/veiculos`        |
| Tipos de Lavagem | `/api/tipos-lavagem`   |
| Agendamentos     | `/api/agendamentos`    |

O login é feito em `POST /api/usuarios/login` (e-mail e senha). A senha nunca é
devolvida nas respostas da API.

Há também a coleção `AutocleanManager.Api.postman_collection.json` para importar
no Postman ou Insomnia.

---

## Estrutura do projeto

```
AutocleanManager.Api/
  Controllers/      endpoints da API
  Models/           entidades e requests
  Data/             DbContext e cálculo de preço
  Migrations/       criação do banco
  wwwroot/          interface web
    login.html       tela de login
    cadastro.html    tela de cadastro
    index.html       sistema (telas por perfil)
    css/estilo.css
    js/api.js        chamadas à API
    js/auth.js       sessão e controle de perfil
    js/app.js        lógica das telas
docker-compose.yml  banco + API
```

---

## Tecnologias

- ASP.NET Core Web API (.NET 10)
- Entity Framework Core + PostgreSQL
- HTML, CSS e JavaScript (sem framework)
- Docker / Docker Compose

Projeto desenvolvido para fins educacionais.
