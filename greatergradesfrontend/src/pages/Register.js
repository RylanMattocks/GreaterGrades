import React, { useContext, useState, useEffect } from 'react';
import { registerAPI, loginAPI } from '../greatergradesapi/Auth';
import { useNavigate } from 'react-router-dom';
import { UserContext } from "../functions/UserContext";
import { getInstitutionsAPI } from '../greatergradesapi/Institutions';

function Register() {
    const { login } = useContext(UserContext);
    const [institutions, setInstitutions] = useState([]);
    const [error, setError] = useState('');
    const [loading, setLoading] = useState(true)
    const navigate = useNavigate();

    const [formData, setFormData] = useState({
        username: '',
        password: '',
        firstName: '',
        lastName: '',
        role: '',
        institutionId: ''
    });


    const handleChange = (e) => {
        setFormData({
            ...formData,
            [e.target.name]: e.target.value
        });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        if (!formData.institutionId) {
            setError('Please select an institution');
            return;
        }
        try {
            const user = await registerAPI(
                formData.username,
                formData.password,
                formData.firstName,
                formData.lastName,
                Number(formData.role),
                Number(formData.institutionId)
            );
            if (user) {
                const token = await loginAPI(formData.username, formData.password)
                if (token) {
                    login(token, user);
                    navigate('/dashboard');
                }
            } else {
                setError('Registration failed. Please try again.');
            }
        } catch (err) {
            console.error(error);
        }
    };

    useEffect(() => {
        const fetchInstitutions = async () => {
            try {
                const data = await getInstitutionsAPI();
                setInstitutions(data || []);
            } catch {
                console.error("Error fetching institutions")
            } finally {
                setLoading(false)
            }
        }
        fetchInstitutions();
    }, [])

    return (
        <div className="login-container">
            <div className="login-box">
                <h2>Register for Greater Grades</h2>
                <form onSubmit={handleSubmit}>
                    <div className="form-group">
                        <label htmlFor="username">Username</label>
                        <input
                            id="username"
                            name="username"
                            type="text"
                            value={formData.username}
                            onChange={handleChange}
                            placeholder='Enter your username'
                            required
                        />
                    </div>
                    <div className="form-group">
                        <label htmlFor="password">Password</label>
                        <input
                            id="password"
                            name="password"
                            type="password"
                            value={formData.password}
                            onChange={handleChange}
                            placeholder='Enter your password'
                            required
                        />
                    </div>
                    <div className="form-group">
                        <label htmlFor="firstName">First Name</label>
                        <input
                            id="firstName"
                            name="firstName"
                            type="text"
                            value={formData.firstName}
                            onChange={handleChange}
                            placeholder='Enter your first name'
                            required
                        />
                    </div>
                    <div className="form-group">
                        <label htmlFor="lastName">Last Name</label>
                        <input
                            id="lastName"
                            name="lastName"
                            type="text"
                            value={formData.lastName}
                            onChange={handleChange}
                            placeholder='Enter your last name'
                            required
                        />
                    </div>
                    <div className="form-group">
                        <label htmlFor="role">Role</label>
                        <select
                            id="role"
                            name="role"
                            value={formData.role}
                            onChange={handleChange}
                            required
                        >
                            <option value="" disabled hidden>Select a role</option>
                            <option value={0}>Student</option>
                            <option value={1}>Teacher</option>
                        </select>
                    </div>
                    <div className="form-group">
                        <label htmlFor="institutionId">Institution</label>
                        <select
                            id="institutionId"
                            name="institutionId"
                            value={formData.institutionId}
                            onChange={handleChange}
                            required
                        >
                            {loading ? (
                                <option value="" disabled hidden>Loading institutions...</option>
                            ) : (
                                <>
                                    {institutions.length > 0 ? (
                                        <>
                                            <option value='' disabled hidden>
                                                Select an institution
                                            </option>
                                                {institutions.map(institution => (
                                                    <option 
                                                        key={institution.institutionId} 
                                                        value={institution.institutionId}
                                                    >
                                                        {institution.name}
                                                    </option>
                                                ))}
                                        </>
                                    ) : (
                                        <option value='' disabled hidden>No instituions found</option>
                                    )}
                                </>
                            )}
                        </select>
                    </div>
                    {error && <div className="error-message">{error}</div>}
                    <button type="submit" className="login-button">Register</button>
                </form>
                <div className="login-link">
                    Already have an account? <a href="/login">Login here</a>
                </div>
            </div>
        </div>
    );
}

export default Register; 