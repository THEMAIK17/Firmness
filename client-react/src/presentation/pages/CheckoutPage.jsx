import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useCart } from '../../application/context/CartContext';
import { useAuth } from '../../application/context/AuthContext';
import { saleRepository } from '../../infraestructure/repositories/saleRepository';
import { Container, Card, ListGroup, Button, Alert } from 'react-bootstrap';

export const CheckoutPage = () => {
    const { cartItems, calculateTotals, clearCart } = useCart();
    const { user } = useAuth();
    const navigate = useNavigate();
    
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');
    const { total } = calculateTotals();

    // 
    const handlePurchase = async () => {
        setLoading(true);
        setError('');

        try {
            
            const saleData = {
                clientId: user.sub || user.id, 
                items: cartItems.map(item => ({
                    productId: item.id,
                    quantity: item.quantity
                }))
            };

       
            await saleRepository.createSale(saleData);

            clearCart();
            alert('¡Compra realizada con éxito! Revisa tu correo para ver el recibo.');
            navigate('/');
        } catch (err) {
            console.error(err); 
            
            const serverMessage = err.response?.data?.message || err.response?.data;
            const displayMessage = serverMessage || 'Ocurrió un error al procesar la compra.';
            
            setError(displayMessage); 
        } finally {
            setLoading(false);
        }
    };

    if (cartItems.length === 0) {
        navigate('/');
        return null;
    }

    return (
        <Container className="py-5" style={{ maxWidth: '600px' }}>
            <h2 className="mb-4">Confirmar Compra</h2>
            
            {error && <Alert variant="danger">{error}</Alert>}

            <Card className="mb-4">
                <Card.Header>Resumen del Pedido</Card.Header>
                <ListGroup variant="flush">
                    {cartItems.map(item => (
                        <ListGroup.Item key={item.id} className="d-flex justify-content-between">
                            <span>{item.name} (x{item.quantity})</span>
                            <span>${(item.unitPrice * item.quantity).toLocaleString()}</span>
                        </ListGroup.Item>
                    ))}
                    <ListGroup.Item className="d-flex justify-content-between fw-bold bg-light">
                        <span>TOTAL A PAGAR</span>
                        <span>${total.toLocaleString()}</span>
                    </ListGroup.Item>
                </ListGroup>
            </Card>

            <div className="d-grid gap-2">
                <Button 
                    variant="success" 
                    size="lg" 
                    onClick={handlePurchase}
                    disabled={loading}
                >
                    {loading ? 'Procesando...' : 'Confirmar y Pagar'}
                </Button>
                <Button variant="outline-secondary" onClick={() => navigate('/cart')}>
                    Volver al Carrito
                </Button>
            </div>
        </Container>
    );
};