import { createContext, useContext, useState, useEffect } from 'react';
import { authRepository } from '../../infraestructure/repositories/authRepository';
import { jwtDecode } from 'jwt-decode'; //read token

const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
    const [user, setUser] = useState(null);
    const [isAuthenticated, setIsAuthenticated] = useState(false);
    const [loading, setLoading] = useState(true); //for session persistence check

    // 1. Check session persistence on app load
    useEffect(() => {
        const checkAuth = () => {
            const token = localStorage.getItem('token');
            if (token) {
                try {
                    const decoded = jwtDecode(token);
                    // Check if token is expired
                    if (decoded.exp * 1000 < Date.now()) {
                        logout();
                    } else {
                        setUser({ ...decoded, token });
                        setIsAuthenticated(true);
                    }
                } catch (error) {
                    console.error("Token inválido", error);
                    logout();
                }
            }
            setLoading(false);
        };
        checkAuth();
    }, []);

    // connect to authRepository for login
    const login = async (email, password) => {
        try {
            
            const data = await authRepository.login(email, password);
            
            // save token in localStorage
            localStorage.setItem('token', data.token);
            
            const decoded = jwtDecode(data.token);
            
            setUser({ ...decoded, token: data.token });
            setIsAuthenticated(true);
            return { success: true };
        } catch (error) {
            console.error("Error en login:", error);
            return { 
                success: false, 
                message: error.response?.data || 'Error al iniciar sesión. Verifique sus credenciales.' 
            };
        }
    };


    const register = async (userData) => {
        try {
            await authRepository.register(userData);
            return { success: true };
        } catch (error) {
            return { 
                success: false, 
                message: error.response?.data || 'Error al registrarse. Intente nuevamente.'
            };
        }
    };

    // logout function
    const logout = () => {
        localStorage.removeItem('token');
        setUser(null);
        setIsAuthenticated(false);
    };

    return (
        <AuthContext.Provider value={{ 
            user, 
            isAuthenticated, 
            login, 
            register, 
            logout,
            loading 
        }}>
            {/* the app is visible only if user had a log in */}
            {!loading && children}
        </AuthContext.Provider>
    );
};

// hook to use auth context
export const useAuth = () => useContext(AuthContext);