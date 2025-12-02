
import { Routes, Route, Navigate } from 'react-router-dom';
import { useAuth } from '../../application/context/AuthContext';
import { LoginPage } from '../pages/LoginPage';
import { RegisterPage } from '../pages/RegisterPage';
import { CatalogPage } from '../pages/CatalogPage';
import { CartPage } from '../pages/CartPage';
import { CheckoutPage } from '../pages/CheckoutPage';

// Component to protect private routes
const PrivateRoute = ({ children }) => {
    const { isAuthenticated, loading } = useAuth();

   if (loading) {
        return <div className="d-flex justify-content-center mt-5">Cargando...</div>;
    }
    
    return isAuthenticated ? children : <Navigate to="/login" />;
};

export const AppRouter = () => {
    return (
        <Routes>
            {/* public route*/}
            <Route path="/login" element={<LoginPage />} />
            <Route path="/register" element={<RegisterPage />} />

            
            {/*private routes */}
            <Route path="/cart" element={
                <PrivateRoute>
                    <CartPage />
                </PrivateRoute>
            } />

            <Route path="/checkout" element={
            <PrivateRoute>
            <CheckoutPage />
            </PrivateRoute>
            } />
            <Route path="/*" element={
                <PrivateRoute>
                    <CatalogPage />
                </PrivateRoute>
            } />
            
        </Routes>
    );
};

