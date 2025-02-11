import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { signUp } from './../../services/authService';
import { toast } from 'react-toastify';
import './SignUp.css';

const SignUp = () => {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [passwordRetype, setPasswordRetype] = useState('');
    const navigate = useNavigate();

    const dataValid = () => {
        const result = username && password && passwordRetype && password == passwordRetype;
        if (!result) toast.error('Неверные данные, проверьте правильность ввода')
        return result;
    }

    const handleSubmit = async (e) => {
        e.preventDefault();
        if (!dataValid()) return;

        try {
          const data = await signUp({ username, password });
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
                <h2 className="form-title">Регистрация</h2>
                <p className="hint-text">Вы не авторизованы. Зарегистрируйтесь, чтобы присоединиться к новой игре</p>
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
                <div className="form-group">
                    <label htmlFor="password">Retype password</label>
                    <input 
                    type="password" 
                    id="password" 
                    name="password" 
                    placeholder="Повторите пароль" 
                    required 
                    onChange={(e) => {setPasswordRetype(e.target.value)}}
                    />
                </div>
                <button type="submit">Отправить</button>
                </form>
                <p className="register-text">
                Есть аккаунт?{' '}
                <Link to="/sign-in" className="register-link">
                    Войти
                </Link>
                </p>
            </div>
        </div>
    </div>
  );
};

export default SignUp;