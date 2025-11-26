import React from 'react';
import { useProducts } from '../../application/hooks/useProducts';
import { useAuth } from '../../application/context/AuthContext';
import { Container, Row, Col, Card, Button, Navbar, Nav } from 'react-bootstrap';
import { useCart } from '../../application/context/CartContext';
import { Link } from 'react-router-dom';

export const CatalogPage = () => {
    const { products, loading, error } = useProducts();
    const { user, logout } = useAuth();
    const { addToCart, cartItems } = useCart();

    if (loading) return <div className="text-center mt-5">Cargando productos...</div>;
    if (error) return <div className="text-center mt-5 text-danger">{error}</div>;

    return (
        <>
            <Navbar bg="dark" variant="dark" expand="lg" className="mb-4">
                <Container>
                    <Navbar.Brand href="#">Firmeza Store</Navbar.Brand>
                    <Navbar.Toggle aria-controls="basic-navbar-nav" />
                    <Navbar.Collapse id="basic-navbar-nav" className="justify-content-end">
                        <Nav>
                            <Nav.Item className="text-white me-3 d-flex align-items-center">
                                Hola, {user?.fullName || 'Cliente'}
                            </Nav.Item>
                            <Nav.Link as={Link} to="/cart" className="text-white me-3">
                            🛒 Carrito ({cartItems.reduce((acc, item) => acc + item.quantity, 0)})
                            </Nav.Link>
                            <Button variant="outline-light" onClick={logout}>Cerrar Sesión</Button>
                        </Nav>
                    </Navbar.Collapse>
                </Container>
            </Navbar>

            <Container>
                <h2 className="mb-4">Catálogo de Productos</h2>
                <Row>
                    {products.map((product) => (
                        <Col key={product.id} md={4} className="mb-4">
                            <Card className="h-100 shadow-sm">
                                <Card.Body className="d-flex flex-column">
                                    <Card.Title>{product.name}</Card.Title>
                                    <Card.Text className="text-muted">
                                        {product.description}
                                    </Card.Text>
                                    <div className="mt-auto">
                                        <h4 className="text-primary mb-3">
                                            ${product.unitPrice.toLocaleString()}
                                        </h4>
                                        <div className="d-grid gap-2">
                                            <Button variant="primary" onClick={() => addToCart(product)}>
                                                Agregar al Carrito
                                            </Button>
                                        </div>
                                    </div>
                                </Card.Body>
                            </Card>
                        </Col>
                    ))}
                </Row>
            </Container>
        </>
    );
};