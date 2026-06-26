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

## A interface

O menu lateral tem cinco telas:

- **Painel** — resumo do dia: agendamentos de hoje, totais e faturamento dos
  serviços finalizados.
- **Agendamentos** — marcar, editar e excluir lavagens. Ao escolher o tipo de
  lavagem e o nível de sujeira, o valor estimado já aparece na hora.
- **Veículos** — cadastrar os carros de cada cliente.
- **Usuários** — cadastrar clientes e equipe (Cliente, Funcionário, Administrador).
- **Tipos de Lavagem** — cadastrar os serviços com preço base e duração.

Cada tela tem um formulário à esquerda e a lista à direita. O botão **Editar**
de uma linha carrega os dados no formulário; **Salvar** grava (cria ou atualiza);
**Excluir** remove.

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
    index.html
    css/estilo.css
    js/api.js        chamadas à API
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
