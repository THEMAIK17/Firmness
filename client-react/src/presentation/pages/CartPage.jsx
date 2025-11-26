import React from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useCart } from '../../application/context/CartContext';
import { Container, Table, Button, Card, Row, Col } from 'react-bootstrap';

export const CartPage = () => {
    const { cartItems, removeFromCart, calculateTotals } = useCart();
    const navigate = useNavigate();

   
    const { subtotal, tax, total } = calculateTotals();

    if (cartItems.length === 0) {
        return (
            <Container className="mt-5 text-center">
                <h2>Tu carrito está vacío </h2>
                <p className="mb-4">Parece que aún no has agregado productos.</p>
                <Link to="/" className="btn btn-primary">Volver al Catálogo</Link>
            </Container>
        );
    }

    return (
        <Container className="py-5">
            <h2 className="mb-4">Carrito de Compras</h2>
            
            <Row>
                <Col md={8}>
                    <Table hover responsive>
                        <thead className="table-light">
                            <tr>
                                <th>Producto</th>
                                <th className="text-center">Cantidad</th>
                                <th className="text-end">Precio Unit.</th>
                                <th className="text-end">Total</th>
                                <th></th>
                            </tr>
                        </thead>
                        <tbody>
                            {cartItems.map((item) => (
                                <tr key={item.id} className="align-middle">
                                    <td>
                                        <span className="fw-bold">{item.name}</span>
                                    </td>
                                    <td className="text-center">{item.quantity}</td>
                                    <td className="text-end">${item.unitPrice.toLocaleString()}</td>
                                    <td className="text-end">
                                        ${(item.unitPrice * item.quantity).toLocaleString()}
                                    </td>
                                    <td className="text-end">
                                        <Button 
                                            variant="outline-danger" 
                                            size="sm"
                                            onClick={() => removeFromCart(item.id)}
                                        >
                                            &times;
                                        </Button>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </Table>
                    <div className="mt-3">
                        <Link to="/" className="btn btn-outline-secondary">
                            &larr; Seguir Comprando
                        </Link>
                    </div>
                </Col>

                <Col md={4}>
                    <Card className="shadow-sm">
                        <Card.Body>
                            <Card.Title className="mb-4">Resumen del Pedido</Card.Title>
                            
                            <div className="d-flex justify-content-between mb-2">
                                <span>Subtotal:</span>
                                <span>${subtotal.toLocaleString()}</span>
                            </div>
                            <div className="d-flex justify-content-between mb-3">
                                <span>Impuestos (19%):</span>
                                <span>${tax.toLocaleString()}</span>
                            </div>
                            <hr />
                            <div className="d-flex justify-content-between mb-4 fw-bold fs-5">
                                <span>Total:</span>
                                <span className="text-primary">${total.toLocaleString()}</span>
                            </div>

                            <Button 
                                variant="success" 
                                className="w-100 py-2"
                                onClick={() => navigate('/checkout')}
                            >
                                Proceder al Pago
                            </Button>
                        </Card.Body>
                    </Card>
                </Col>
            </Row>
        </Container>
    );
};