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
        console.log("1. Iniciando chequeo de sesión..."); // <--- LOG 1
        const checkAuth = () => {
            const token = localStorage.getItem('token');
            console.log("2. Token encontrado en Storage:", token); // <--- LOG 2

            if (token) {
                try {
                    const decoded = jwtDecode(token);
                    console.log("3. Token decodificado:", decoded); // <--- LOG 3
                    
                   
                    const expirationTime = decoded.exp * 1000;
                    const currentTime = Date.now();
                    console.log(`4. Expiración: ${expirationTime} vs Ahora: ${currentTime}`); // <--- LOG 4

                    if (expirationTime < currentTime - 300000) { 
                        console.log("Token expirado hace mucho. Cerrando sesión.");
                        logout();
                    } else {
                        console.log("6. Token válido. Restaurando sesión."); // <--- LOG 6
                        setUser({ ...decoded, token });
                        setIsAuthenticated(true);
                    }
                } catch (error) {
                    console.error("7. Error al decodificar token:", error); // <--- LOG 7
                    logout();
                }
            } else {
                console.log("8. No hay token. Usuario anónimo."); // <--- LOG 8
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