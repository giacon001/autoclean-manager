// Simulação de banco de dados em memória
let users = [
    { id: 1, name: 'João Silva', email: 'joao@email.com' },
    { id: 2, name: 'Maria Souza', email: 'maria@email.com' }
];

// GET /users- Listar todos
export const getAllUsers = (req, res) => {
    res.json(users);
};

// GET /users/:id- Buscar por ID
export const getUserById = (req, res) => {
    const id = parseInt(req.params.id);
    const user = users.find(u => u.id === id);

    if (!user) {
        return res.status(404).json({ message: 'Usuário não encontrado' });
    }

    res.json(user);
};

// POST /users- Criar novo
export const createUser = (req, res) => {
    const { name, email } = req.body;
    if (!name || !email) {
        return res.status(400).json({ message: 'Nome e email são obrigatórios' });
    }

    const newUser = {
        id: users.length + 1,
        name,
        email
    };

    users.push(newUser);
    res.status(201).json(newUser);
};

// PUT /users/:id- Atualizar
export const updateUser = (req, res) => {
    const id = parseInt(req.params.id);
    const { name, email } = req.body;
    const userIndex = users.findIndex(u => u.id === id);
    if (userIndex ===-1) {
        return res.status(404).json({ message: 'Usuário não encontrado' });
    }

    users[userIndex] = { ...users[userIndex], name, email };
    res.json(users[userIndex]);
};

// DELETE /users/:id- Remover
export const deleteUser = (req, res) => {
    const id = parseInt(req.params.id);
    const userIndex = users.findIndex(u => u.id === id);
    if (userIndex ===-1) {
        return res.status(404).json({ message: 'Usuário não encontrado' });
    }
    
    users.splice(userIndex, 1);
    res.status(204).send(); // No content
};