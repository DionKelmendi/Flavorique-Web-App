import logo from './logo.svg';
import React, { useState, useEffect } from 'react';
import Header from './components/header';
import './App.css';

function App() {

  const [userData, setUserData] = useState(null);
  const [error, setError] = useState(null);

  useEffect(() => {
    fetch('https://localhost:7147/api/Account/user', {
      method: 'GET',
      credentials: 'include'
    })
      .then(response => {
        if (!response.ok) {
          throw new Error(`Error! Status: ${response.status}`);
        }
        return response.json();
      })
      .then(data => {
        console.log("Setting user data");
        setUserData(data);
      })
      .catch(error => {
        setError(error.message);
        console.log(error);
      });
  }, []);

  return (
    <>
      <Header userData={userData} />
    </>
  );
}

export default App;
