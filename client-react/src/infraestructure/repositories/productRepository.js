
import { api } from '../api/axiosConfig.js';

export const productRepository = {
    
    getAll: async () => {
        const response = await api.get('/Products');
        return response.data;
    },

    getById: async (id) => {
        const response = await api.get(`/Products/${id}`);
        return response.data;
    }
};