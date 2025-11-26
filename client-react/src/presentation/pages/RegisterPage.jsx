import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useAuth } from '../../application/context/AuthContext';
import { Container, Card, Form, Button, Alert, Row, Col } from 'react-bootstrap';

export const RegisterPage = () => {
    const [formData, setFormData] = useState({
        email: '',
        password: '',
        firstName: '',
        lastName: '',
        documentNumber: '',
        address: '',
        phoneNumber: ''
    });
    const [error, setError] = useState('');
    const [successMsg, setSuccessMsg] = useState('');
    
    const { register } = useAuth();
    const navigate = useNavigate();

    const handleChange = (e) => {
        setFormData({
            ...formData,
            [e.target.name]: e.target.value
        });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError('');
        setSuccessMsg('');
        
        const result = await register(formData);
        
        if (result.success) {
            setSuccessMsg('Registro exitoso! Redirigiendo al login...');
            setTimeout(() => {
                navigate('/login');
            }, 2000);
        } else {
           
            const msg = result.message?.response?.data || 'Error al registrarse';
            setError(JSON.stringify(msg));
        }
    };

    return (
        <Container className="d-flex align-items-center justify-content-center py-5">
            <div className="w-100" style={{ maxWidth: "600px" }}>
                <Card>
                    <Card.Body>
                        <h2 className="text-center mb-4">Crear Cuenta</h2>
                        {error && <Alert variant="danger">{error}</Alert>}
                        {successMsg && <Alert variant="success">{successMsg}</Alert>}
                        
                        <Form onSubmit={handleSubmit}>
                            <Row>
                                <Col md={6}>
                                    <Form.Group className="mb-3">
                                        <Form.Label>Nombre</Form.Label>
                                        <Form.Control name="firstName" required onChange={handleChange} />
                                    </Form.Group>
                                </Col>
                                <Col md={6}>
                                    <Form.Group className="mb-3">
                                        <Form.Label>Apellido</Form.Label>
                                        <Form.Control name="lastName" required onChange={handleChange} />
                                    </Form.Group>
                                </Col>
                            </Row>

                            <Form.Group className="mb-3">
                                <Form.Label>Documento</Form.Label>
                                <Form.Control name="documentNumber" required onChange={handleChange} />
                            </Form.Group>

                            <Form.Group className="mb-3">
                                <Form.Label>Email</Form.Label>
                                <Form.Control type="email" name="email" required onChange={handleChange} />
                            </Form.Group>

                            <Form.Group className="mb-3">
                                <Form.Label>Contraseña</Form.Label>
                                <Form.Control type="password" name="password" required onChange={handleChange} />
                            </Form.Group>

                            <Row>
                                <Col md={6}>
                                    <Form.Group className="mb-3">
                                        <Form.Label>Teléfono</Form.Label>
                                        <Form.Control name="phoneNumber" onChange={handleChange} />
                                    </Form.Group>
                                </Col>
                                <Col md={6}>
                                    <Form.Group className="mb-3">
                                        <Form.Label>Dirección</Form.Label>
                                        <Form.Control name="address" onChange={handleChange} />
                                    </Form.Group>
                                </Col>
                            </Row>

                            <Button className="w-100 mt-3" type="submit">
                                Registrarse
                            </Button>
                        </Form>
                    </Card.Body>
                </Card>
                <div className="w-100 text-center mt-2">
                    ¿Ya tienes cuenta? <Link to="/login">Inicia Sesión</Link>
                </div>
            </div>
        </Container>
    );
};