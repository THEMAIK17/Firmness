import React from 'react';
import ReactDOM from 'react-dom/client';
import App from './App.jsx';

//global styles
import 'bootstrap/dist/css/bootstrap.min.css';
import './index.css'; 

// import AuthProvider to wrap the app
import { AuthProvider } from './application/context/AuthContext';

ReactDOM.createRoot(document.getElementById('root')).render(
  <React.StrictMode>
    <AuthProvider>
      <App />
    </AuthProvider>
  </React.StrictMode>,
);