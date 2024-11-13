import { useEffect, useState, useContext } from "react";
import { UserContext } from '../functions/UserContext';

const url = 'http://localhost:5000/api/Institutions/'
const getCommonHeader = (token) => ({
    headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
    },
});

export const useGetAllInstitutions = (refreshTrigger) => {
    const [institutions, setInstitutions] = useState([])
    const { authToken } = useContext(UserContext);

    useEffect(() => {
        const fetchInstitutions = async () => {
            try {
                const response = await fetch(`${url}`, getCommonHeader(authToken));
                const data = await response.json();
                setInstitutions(data || []);
            } catch {
                console.error("Error fetching institutions")
            }
        }

        if (authToken) {
            fetchInstitutions();
        }

    }, [authToken, refreshTrigger]);

    return institutions;
}


export const addInstitution = async (name, address, authToken) => {
    try {
        const response = await fetch(`${url}`, {
            method: 'POST',
            headers: {
                'Authorization': `Bearer ${authToken}`,
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ name, address })
        })
        return await response.json();
    } catch (error) {
        console.error('Error adding institution:', error)
        return null;
    }
}


export const useGetInstitutionById = (id) => {
    const [institution, setInstitution] = useState({})
    const [loading, setLoading] = useState(true);
    const { authToken } = useContext(UserContext);

    useEffect(() => {
        const fetchInstitution = async () => {
            try {
                const response = await fetch(`${url}${id}`, getCommonHeader(authToken));
                const data = await response.json();
                setInstitution(data || {});
            } catch {
                console.error("Error fetching institution")
            } finally {
                setLoading(false);
            }
        }
        if (authToken && id) fetchInstitution();
    }, [authToken, id])
    return {institution, loading};
}


export const updateInstitution = async (id, name, authToken) => {
    try {
        const response = await fetch(`${url}${id}`, {
            method: 'PUT',
            headers: {
                'Authorization': `Bearer ${authToken}`,
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ name })
        });
        if (response.status !== 204) throw new Error();
        return 'Updated'
    } catch (error) {
        console.error("Error updating institution:", error)
        return null;
    }
}


export const deleteInstitution = async (id, authToken) => {
    try {
        const response = await fetch(`${url}${id}`, {
            method: 'DELETE',
            headers: {
                'Authorization': `Bearer ${authToken}`,
                'Content-Type': 'application/json'
            }
        });
        if (response.status !== 204) throw new Error();
        return 'Deleted';
    } catch {
        console.error("Error deleting institution")
        return null;
    }
}


//// non hook functions

export const getInstitutionsAPI = async () => {
    try {
        const response = await fetch(`${url}`);
        const data = await response.json();
        return data || null;
    } catch {
        console.error("Error fetching institutions")
        return null;
    }
}