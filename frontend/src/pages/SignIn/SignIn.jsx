import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { signIn } from './../../services/authService';
import { toast } from 'react-toastify';
import './SignIn.css';

const SignIn = () => {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const navigate = useNavigate();

    const dataValid = () => {
        const result = username && password;
        if (!result) toast.error('Неверные данные, проверьте правильность ввода')
        return result;
    }

    const handleSubmit = async (e) => {
        e.preventDefault();
        if (!dataValid()) return;

        try {
            const data = await signIn({ username, password });
            if (data) {
            navigate('/');
            }
        } catch (error) {}
    };
    
    return (
    <div>
        <header className="header">
        <Link to="/" className="logo">
            Rock<br/>Paper<br/>Scissors
        </Link>
        </header>
        <div className="signin-page">
            <div className="form-container">
                <h2 className="form-title">Вход</h2>
                <p className="hint-text">Вы не авторизованы. Войдите, чтобы присоединиться к новой игре</p>
                <form className="signin-form" onSubmit={handleSubmit}>
                <div className="form-group">
                    <label htmlFor="username">Username</label>
                    <input 
                    type="text" 
                    id="username" 
                    name="username" 
                    placeholder="Введите username" 
                    required 
                    onChange={(e) => {setUsername(e.target.value)}}
                    />
                </div>
                <div className="form-group">
                    <label htmlFor="password">Password</label>
                    <input 
                    type="password" 
                    id="password" 
                    name="password" 
                    placeholder="Введите пароль" 
                    required 
                    onChange={(e) => {setPassword(e.target.value)}}
                    />
                </div>
                <button type="submit">Войти</button>
                </form>
                <p className="register-text">
                Нет аккаунта?{' '}
                <Link to="/sign-up" className="register-link">
                    Зарегистрироваться
                </Link>
                </p>
            </div>
        </div>
    </div>
  );
};

export default SignIn;