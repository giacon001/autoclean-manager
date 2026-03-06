# Autoclean-manager
Autoclean-manager é um sistema de gerenciamento e agendamento para lava-jatos.  
A plataforma permite que clientes cadastrem seus veículos, agendem lavagens e acompanhem o status do serviço, enquanto o administrador controla a agenda e o fluxo de atendimento.

O objetivo do projeto é demonstrar a construção de um sistema completo com autenticação, gerenciamento de dados e controle operacional.

---

# Funcionalidades

## 1. Gestão de Usuários e Veículos

### Cadastro e Login
Sistema de autenticação para clientes utilizando e-mail e senha.

### Minha Garagem
Área onde o usuário pode cadastrar e gerenciar seus veículos.

Dados armazenados por veículo:

- Marca
- Modelo
- Placa
- Cor

Um usuário pode cadastrar múltiplos veículos.

### Perfil do Usuário
O cliente pode:

- Atualizar dados de contato
- Visualizar histórico de lavagens realizadas
- Gerenciar veículos cadastrados

---

## 2. Sistema de Agendamento

### Seleção de Veículo
O cliente escolhe qual veículo cadastrado será utilizado no agendamento.

### Configuração da Lavagem

**Escopo da lavagem**
- Interna
- Externa
- Completa

**Nível de sujeira**
- Leve
- Média
- Pesada

### Seletor de Data e Hora
Calendário dinâmico que apresenta apenas horários disponíveis para agendamento.

### Resumo e Confirmação
Tela final exibindo:

- Veículo selecionado
- Tipo de lavagem
- Nível de sujeira
- Data e horário

O agendamento inicia com status:

- `Aguardando`
- `Confirmado`

---

## 3. Painel Administrativo

### Agenda do Dia
Visualização de todos os carros agendados organizados por horário.

### Gestão de Status
O administrador pode atualizar o estado da lavagem:

- Na fila
- Lavando
- Pronto para retirada
- Finalizado

### Fila de Espera
Lista de agendamentos futuros para organização da equipe e controle de demanda.

---

## Objetivo do Projeto

Este projeto foi criado para estudo e demonstração de:

- autenticação de usuários
- modelagem de banco de dados
- agendamento de serviços
- controle de fluxo operacional
- construção de um painel administrativo

---

## Possíveis Melhorias Futuras

- Pagamento online
- Notificações por WhatsApp ou e-mail
- Sistema de cupons e promoções
- Dashboard com métricas de faturamento
- Aplicativo mobile
