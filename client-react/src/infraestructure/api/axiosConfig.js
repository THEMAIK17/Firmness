import axios from 'axios';

// read the API base URL from environment variables
const API_BASE_URL = import.meta.env.VITE_API_URL;

// Create an Axios instance with default configuration
const api = axios.create({
    baseURL: API_BASE_URL.endsWith('/') ? API_BASE_URL : `${API_BASE_URL}/`,
    headers: {
        'Content-Type': 'application/json',
    },
});

// Add a request interceptor to include the token in headers if it exists
api.interceptors.request.use(
    (config) => {
        // If the URL starts with a slash, Axios treats it as absolute to the domain.
        // We remove it to make it relative to the baseURL (maintaining the /api/ prefix).
        if (config.url && config.url.startsWith('/')) {
            config.url = config.url.substring(1);
        }
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