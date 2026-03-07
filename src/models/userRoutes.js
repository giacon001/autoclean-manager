import express from 'express';
import {
    getAllUsers,
    getUserById,
    createUser,
    updateUser,
    deleteUser
} from '../controllers/userControllers.js';

const router = express.Router();

// Definindo as rotas
router.get('/', getAllUsers);
router.get('/:id', getUserById);
router.post('/', createUser);
router.put('/:id', updateUser);
router.delete('/:id', deleteUser);

export default router;