import axios from 'axios';

// read the API base URL from environment variables
const API_BASE_URL = import.meta.env.VITE_API_URL;

// Create an Axios instance with default configuration
const api = axios.create({
    baseURL: API_BASE_URL,
    headers: {
        'Content-Type': 'application/json',
    },
});

// Add a request interceptor to include the token in headers if it exists
api.interceptors.request.use(
    (config) => {
       
        const token = localStorage.getItem('token');
        
        if (token) {
            
            config.headers['Authorization'] = `Bearer ${token}`;
        }
        return config;
    },
    (error) => {
        return Promise.reject(error);
    }
);

export default api;