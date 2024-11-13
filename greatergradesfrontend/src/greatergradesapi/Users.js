import { useEffect, useState, useContext } from "react";
import { UserContext } from '../functions/UserContext';

const url = 'http://localhost:5000/api/Auth';

export const useGetAllUsers = () => {
    const [users, setUsers] = useState([]);
    const { authToken } = useContext(UserContext);

    useEffect(() => {
        const fetchUsers = async () => {
            try {
                const response = await fetch(url, {
                    headers: {
                        'Authorization': `Bearer ${authToken}`,
                        'Content-Type': 'application/json'
                    }
                });
                if (!response.ok) throw new Error('Failed to fetch users');
                const data = await response.json();
                // Filter out admin users (role > 1) and sort by role
                const filteredUsers = data
                    .filter(user => user.role <= 1)
                    .sort((a, b) => a.role - b.role);
                setUsers(filteredUsers);
            } catch (error) {
                console.error('Error fetching users:', error);
                setUsers([]);
            }
        };

        if (authToken) {
            fetchUsers();
        }
    }, [authToken]);

    return users;
}; 