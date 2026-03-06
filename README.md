# AutoClean Manager

AutoClean Manager é um sistema completo para gerenciamento de lava-jatos. A plataforma permite que clientes cadastrem seus veículos, agendem lavagens e acompanhem o status do serviço em tempo real, enquanto a equipe administrativa gerencia a agenda, controla o fluxo de atendimento e monitora métricas operacionais.

O objetivo do projeto é demonstrar a construção de um sistema realista de gerenciamento de serviços, incluindo autenticação de usuários, modelagem de banco de dados, controle de agenda, gestão operacional e painel administrativo.

---

# Principais Funcionalidades

## 1. Gestão de Usuários

### Cadastro e Autenticação

Sistema de autenticação baseado em e-mail e senha.

Funcionalidades:

- cadastro de novos usuários
- login e logout
- recuperação de senha
- proteção de rotas autenticadas

### Perfil do Usuário

O cliente pode:

- atualizar dados pessoais
- alterar senha
- visualizar histórico de lavagens
- gerenciar veículos cadastrados

---

# 2. Gestão de Veículos (Minha Garagem)

Cada usuário possui uma área chamada **Minha Garagem**, onde pode cadastrar e gerenciar seus veículos.

Dados armazenados:

- Marca
- Modelo
- Placa
- Cor
- Ano (opcional)

Funcionalidades:

- adicionar veículo
- editar veículo
- remover veículo
- listar todos os veículos do usuário

Um usuário pode possuir **múltiplos veículos cadastrados**.

---

# 3. Sistema de Agendamento de Lavagens

Este é o núcleo do sistema.

### Seleção de Veículo

O cliente escolhe qual veículo cadastrado será utilizado no agendamento.

### Configuração da Lavagem

#### Tipo de Lavagem

- Lavagem externa
- Lavagem interna
- Lavagem completa

Cada tipo possui:

- preço base
- duração estimada

#### Nível de Sujeira

Define o grau de trabalho necessário.

Opções:

- Leve
- Média
- Pesada

O nível de sujeira pode influenciar:

- tempo de execução
- preço final

### Cálculo Automático de Preço

O sistema calcula automaticamente o valor do serviço baseado em:

- tipo de lavagem
- nível de sujeira

Exemplo:

```
Lavagem Completa: R$60
Sujeira Pesada: +20%

Total: R$72
```

### Seleção de Data e Horário

O cliente seleciona o horário através de um calendário dinâmico.

O sistema:

- exibe apenas horários disponíveis
- bloqueia horários já ocupados
- respeita a duração do serviço

### Resumo do Agendamento

Antes da confirmação o sistema mostra:

- veículo selecionado
- tipo de lavagem
- nível de sujeira
- data e horário
- valor total do serviço

Status inicial do agendamento:

- Aguardando
- Confirmado

---

# 4. Cancelamento e Reagendamento

O cliente pode:

- cancelar um agendamento
- reagendar para outro horário disponível

Regras podem incluir:

- cancelamento permitido até determinado tempo antes do horário marcado

---

# 5. Histórico de Serviços

O usuário pode visualizar todas as lavagens já realizadas.

Informações exibidas:

- veículo
- data
- tipo de lavagem
- status
- valor pago

---

# 6. Painel Administrativo

Área exclusiva para gestão operacional do lava-jato.

### Agenda do Dia

Visualização completa dos agendamentos do dia.

Exibe:

- horário
- veículo
- cliente
- tipo de lavagem
- status atual

### Gestão de Status da Lavagem

A equipe pode atualizar o progresso do serviço.

Fluxo padrão:

```
Aguardando
↓
Na fila
↓
Lavando
↓
Pronto para retirada
↓
Finalizado
```

### Fila de Espera

Lista de todos os agendamentos futuros.

Permite:

- planejamento de equipe
- previsão de demanda
- organização do fluxo de trabalho

### Busca de Veículos

Ferramenta para localizar veículos rapidamente.

Busca por:

- placa
- cliente

Permite visualizar histórico de lavagens daquele veículo.

---

# 7. Sistema de Notificações

O sistema pode enviar notificações para o cliente em eventos importantes.

Exemplos:

- confirmação de agendamento
- lembrete antes do horário da lavagem
- aviso de serviço finalizado

Notificações podem ser implementadas via:

- e-mail
- SMS
- WhatsApp (integração futura)

---

# 8. Dashboard Administrativo

Painel com métricas operacionais do negócio.

Exemplos de indicadores:

- número de lavagens por dia
- faturamento diário
- tipo de lavagem mais solicitado
- horários de maior movimento

---

# 9. Sistema de Papéis de Usuário

O sistema suporta diferentes níveis de acesso.

### Cliente

- gerenciar veículos
- agendar lavagens
- acompanhar status
- visualizar histórico

### Funcionário

- visualizar agenda
- atualizar status das lavagens

### Administrador

- acesso total ao sistema
- gestão de agenda
- controle de usuários
- visualização de métricas

---

# 10. Possíveis Expansões Futuras

Funcionalidades que podem ser adicionadas futuramente:

- pagamento online
- cupons e promoções
- sistema de fidelidade
- aplicativo mobile
- integração com APIs de pagamento
- notificações por WhatsApp
- integração com sistemas de gestão financeira

---

# Objetivo do Projeto

Este projeto foi desenvolvido para estudo e demonstração de conceitos como:

- autenticação de usuários
- modelagem de banco de dados
- arquitetura de APIs
- sistemas de agendamento
- controle de fluxo operacional
- construção de painéis administrativos

---

# Estrutura Conceitual do Sistema

Entidades principais do sistema:

```
Users
Vehicles
Appointments
Services
WashStatus
Payments (futuro)
```

Relacionamento simplificado:

```
User
 └─ Vehicles
     └─ Appointments
         └─ Service
```

---

# Licença

Projeto desenvolvido para fins educacionais.
