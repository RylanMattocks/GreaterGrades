import { useEffect, useState, useContext } from "react";
import { UserContext } from '../functions/UserContext';

const url = 'http://localhost:5000/api/Grades/'
const getCommonHeader = (token) => ({
    headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
    },
});

export const useGetGrades = (refresh) => {
    const [grades, setGrades] = useState([])
    const { authToken } = useContext(UserContext);

    useEffect(() => {
        const fetchGrades = async () => {
            try {
                const response = await fetch(`${url}`, getCommonHeader(authToken));
                const data = await response.json();
                setGrades(data || [])
            } catch {
                console.error('Error fetching grades');
            }
        }
        if (authToken) fetchGrades();
    }, [authToken, refresh])
    return grades;
}



export const getGrade = async (id, authToken) => {
    try {
        const response = await fetch(`${url}${id}`, getCommonHeader(authToken));
        return await response.json();
    } catch {
        console.error('Error fetching grade');
        return null;
    }
}


export const updateGrade = async (id, score, gradingStatus, authToken) => {
    try {
        const response = await fetch(`${url}${id}`, {
            method: 'PUT',
            headers: {
                'Authorization': `Bearer ${authToken}`,
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ score, gradingStatus })
        })
        if (response.status !== 204) throw new Error();
    } catch {
        console.error('Failed to update user');
    }
}







export const getGrades = async (authToken) => {
    try {
        const response = await fetch(`${url}`, getCommonHeader(authToken));
        return await response.json()
    } catch {
        console.error('Error fetching grades');
        return null;
    }
}