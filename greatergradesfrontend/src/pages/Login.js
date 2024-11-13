import React, { useState, useContext } from 'react';
import { UserContext } from '../functions/UserContext';
import { loginAPI, getUserAPI } from '../greatergradesapi/Auth';
import { useNavigate } from 'react-router-dom';

function Login() {
    const { login } = useContext(UserContext);
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');
    const navigate = useNavigate();

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            const token = await loginAPI(username, password);
            if (!token) {
                setError('Invalid credentials. Please try again.');
                throw new Error(error)
            }
            
            const user = await getUserAPI(username, token);
            if (!user) {
                setError('Error fetching user data.');
                throw new Error(error)
            }
            
            login(token, user);
            navigate('/dashboard');
        } catch (err) {
            console.error(err)
        }
    };

    return (
        <div className="login-container">
            <div className="login-box">
                <h2>Welcome to Greater Grades</h2>
                <form onSubmit={handleSubmit}>
                    <div className="form-group">
                        <label htmlFor="username">Username</label>
                        <input 
                            id="username"
                            type="text" 
                            value={username} 
                            onChange={(e) => setUsername(e.target.value)} 
                            placeholder="Enter your username" 
                            required 
                        />
                    </div>
                    <div className="form-group">
                        <label htmlFor="password">Password</label>
                        <input 
                            id="password"
                            type="password" 
                            value={password} 
                            onChange={(e) => setPassword(e.target.value)} 
                            placeholder="Enter your password" 
                            required 
                        />
                    </div>
                    {error && <div className="error-message">{error}</div>}
                    <button type="submit" className="login-button">Login</button>
                </form>
                <div className="login-link">
                    Don't have an account? <a href="/register">Register here</a>
                </div>
            </div>
        </div>
    );
}

export default Login; 