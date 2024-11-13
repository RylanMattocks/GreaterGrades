import { useEffect, useState, useContext } from "react";
import { UserContext } from '../functions/UserContext';

const url = 'http://localhost:5000/api/Auth/';
const getCommonHeader = (token) => ({
    headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
    },
});


export const useRegister = (username, password, firstName, lastName, role, institutionId) =>{
    const [user, setUser] = useState({})
    const { authToken } = useContext(UserContext);

    useEffect(() => {
        const fetchRegister = async () => {
            try{
                const response = await fetch(`${url}register`, {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({ username, password, firstName, lastName, role, institutionId })
                });
                const data = await response.json();
                setUser(data || {})
                if (response.status !== 204) throw new Error();
            } catch {
                console.error("Failed to register user")
            }
        }
        if (authToken && username && password && firstName && lastName && role && institutionId) fetchRegister();
    }, [authToken, username, password, firstName, lastName, role, institutionId])
    return user;
}


export const useLogin = (username, password) => {
    const [token, setToken] = useState("");

    useEffect(() => {
        const fetchToken = async () => {
            try {
                const response = await fetch(`${url}login`, {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({ username, password })
                });
                const data = await response.json();
                setToken(data?.token || '')
            } catch (error) {
                console.error("Error fetching Token: ", error.message)
            }
        }
        if (username && password) fetchToken();
    }, [username, password]);
    return token;
}


export const useGetAllUsers = () => {
    const [users, setUsers] = useState([]);
    const { authToken } = useContext(UserContext);
    
    useEffect(() => {
        const fetchUsers = async () => {
            try{
                const response = await fetch(`${url}`, getCommonHeader(authToken))
                const data = await response.json();
                setUsers(data || [])
            } catch{
                console.error("Error fetching users");
            }
        }
        if (authToken) fetchUsers();
    }, [authToken])
    return (users)
}


export const useUpdateUser = (id, firstName, lastName) => {
    const { authToken } = useContext(UserContext);
    useEffect(() => {
        const fetchUpdateUser = async () => {
            try {
                const response = await fetch(`${url}${id}`, {
                    method: 'PUT',
                    headers: {
                        'Authorization': `Bearer ${authToken}`,
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({ firstName, lastName })
                })
                if (response.status !== 204) throw new Error();
            } catch {
                console.error('Error updating user')
            }
        }
        if (authToken && firstName && lastName) fetchUpdateUser();
    }, [authToken, id, firstName, lastName])
}


export const useGetUserById = (id) => {
    const [user, setUser] = useState({});
    const { authToken } = useContext(UserContext);

    useEffect(() => {
        const fetchUserById = async () => {
            try{
                const response = await fetch(`${url}id/${id}`, getCommonHeader(authToken))
                const data = await response.json();
                setUser(data || {});
            } catch (error){
                console.error("Error fetching user: " + error.message);
            }
        }
        if (authToken && id) fetchUserById();
    }, [authToken, id]);
    return (user);
};


export const useGetUserByUsername = (username) => {
    const [user, setUser] = useState({});
    const { authToken } = useContext(UserContext);

    useEffect(() => {
        const fetchUserByUsername = async () => {
            try{
                const response = await fetch(`${url}username/${username}`, getCommonHeader(authToken))
                const data = await response.json();
                setUser(data || {});
            } catch{
                console.error("Error fetching user");
            }
        }
        if (authToken && username) fetchUserByUsername();
    }, [authToken, username]);
    return (user);
}

export const getUserById = async (id, authToken) => {
    try{
        const response = await fetch(`${url}id/${id}`, getCommonHeader(authToken));
        return await response.json();
    } catch (error){
        return null;
    };
};


//// Non hook functions for handling login process

export const loginAPI = async (username, password) => {
    try {
        const response = await fetch(`${url}login`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ username, password })
        });
        const data = await response.json();
        return data?.token || null;
    } catch {
        console.error("Error fetching token");
        return null;
    }
};

export const getUserAPI = async (username, token) => {
    try {
        const response = await fetch(`${url}username/${username}`, getCommonHeader(token));
        const data = await response.json();
        return data || null;
    } catch {
        console.error('Error fetching user');
        return null;
    }
}

export const registerAPI = async (username, password, firstName, lastName, role, institutionId) => {
    try{
        const response = await fetch(`${url}register`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ username, password, firstName, lastName, role, institutionId })
        });
        const data = await response.json();
        return data || null;
    } catch {
        console.error("Failed to register user")
        return null;
    }
}