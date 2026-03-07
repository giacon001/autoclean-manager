import express from 'express';
import userRoutes from '../autoclean-manager/src/models/userRoutes.js';

const app = express();
const port = 3000;

// Middleware para parsear JSON
app.use(express.json());

// Rotas
app.use('/users', userRoutes);

// Rota inicial
app.get('/', (req, res) => {
    res.send('Bem-vindo ao AutoClean Manager!');
});

app.listen(port, () => {
    console.log(`Servidor rodando em http://localhost:${port}`);
});