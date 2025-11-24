import { api } from '../api/axiosConfig.js';

export const authRepository = {
    // function to log in
    login: async (email, password) => {
        
        const response = await api.post('/Auth/Login', { email, password });
        return response.data;
    },

    // function to register
    register: async (userData) => {
        const response = await api.post('/Clients', userData);
        return response.data;
    }
};