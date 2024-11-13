import { useEffect, useState, useContext } from "react";
import { UserContext } from '../functions/UserContext';

const url = 'http://localhost:5000/api/Assignments/'
const getCommonHeader = (token) => ({
  headers: {
    'Authorization': `Bearer ${token}`,
    'Content-Type': 'application/json'
  },
});

export const useGetAllAssignments = (refresh) => {
  const [assignments, setAssignments] = useState([])
  const { authToken } = useContext(UserContext);
  
  useEffect(() => {
    const fetchAssignment = async () => {
      try {
        const response = await fetch(`${url}`, getCommonHeader(authToken))
        const data = await response.json();
        setAssignments(data || [])
      } catch {
        console.error("Error fetching assignments")
      }
    }
    if (authToken) fetchAssignment();
  }, [authToken, refresh])
  return assignments;
}


export const addAssignment = async (name, classId, maxScore, authToken) => {
      try {
        const response = await fetch(`${url}`, {
          method: 'POST',
          headers: {
            'Authorization': `Bearer ${authToken}`,
            'Content-Type': 'application/json'
            },
          body: JSON.stringify({ name, classId, maxScore })
        })
        return response.json();
      } catch {
        console.error("Error adding assignment")
        return null;
      }
}


export const useGetAssignmentById = (id, refresh) => {
  const [assignment, setAssignment] = useState({})
  const { authToken } = useContext(UserContext);
  
  useEffect(() => {
    const fetchAssignment = async () => {
      try {
        const response = await fetch(`${url}${id}`, getCommonHeader(authToken))
        const data = response.json();
        setAssignment(data || {})
      } catch {
        console.error("Error fetching assignments")
      }
    }
    if (authToken) fetchAssignment();
  }, [authToken, id, refresh])
  return assignment;
}


export const getAssignmentById = async (id, authToken) => {
  try {
    const response = await fetch(`${url}${id}`, getCommonHeader(authToken))
    return await response.json();
  } catch {
    console.error("Error fetching assignment")
    return null;
  }
}


export const updateAssignment = async (id, name, classId, authToken) => {
  try {
    const response = await fetch(`${url}${id}`, {
      method: 'PUT',
      headers: {
        'Authorization': `Bearer ${authToken}`,
        'Content-Type': 'application/json'
        },
      body: JSON.stringify({ name, classId })
    })
    if (response.status !== 204) throw new Error;
  } catch {
    console.error("Error updating assignment")
  }
}

export const deleteAssignment = async (id, authToken) => {
  try {
    const response = await fetch(`${url}${id}`, {
      method: 'DELETE',
      headers: {
        'Authorization': `Bearer ${authToken}`,
        'Content-Type': 'application/json'
      }
    })
    if (response.status !== 204) throw new Error;
    return "Deleted";
  } catch {
    console.error("Error deleting assignment")
    return null;
  }
}



//// Fetches not specifially tied to a single endpoint

