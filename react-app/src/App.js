import logo from './logo.svg';
import React, { useState, useEffect } from 'react';
import './App.css';

function App() {

  useEffect(() => {
    const fetchData = async () => {
      try {
        const response = await fetch('https://localhost:7147/api/Account/isUserSignedIn', {
          method: 'GET',
          headers: {
            'Content-Type': 'application/json',
            'Accept': 'application/json',
            'Cookie': '.AspNetCore.Identity.Application=CfDJ8H-dB-qDSvVCpMPza4zPH0cGfD0Ff6h8YsIu_2usKiZ-FjTb5Uik8-ph7CaJVYPzFGJAPZpQaoVH1JLHWANCxR9w2a3djpr0gcJJ_zxnEXQ6R-fSfeAfpThNbDtyP8UfdsSkk2YbvR5TBHE4vaFWmfJAwCoKp8iRqCQbS0JjAe2RgPCpSbuuXkLE_zq2zj5saLeCWMZWhWtsx-q9j5iLMn-12b1YqM_-1VsOMKvg90ov_1U-dLMHQ6bkMrKMsjNlA9o_qEXJSqUxH-vADsFbSA4A5AfPVd6fvFnWn2pTu7flZQ-hxOaZgxWPW-gFc8_6lEfKbVoegUNkSc-gCO9PBco13toN35mZ2SAGHYgL115-2PlP8K8isNN_lkkqQBKzzxsbXHTIp6b8nuenIkaA_ma9KXMO_9iOs9BQ9i0Be_4Sns_EIIq-eMi59wY81BZyR0rvZiZnX16NCYWV6LC0gh8ifcqVTlS_Bl582lEMp3FVNi2uupy2p17HPtU8ZoCnj004wLVLPwv7ZmGvEv1IBagDDfPQikuHrBcrG8wPATdix62AKJSsXeaZ-2mJ3tMWmld-agTPF8q1TKqC3WY0Femjn7r6seAfu9y64r2_nsJfJvOLltZA6zxcvgBfyqi-b_q-6BgRuix9Ku3qlIqe0ld4Tzmrh3B4mGAxeeHOrn5ikLeroTNd9GuIRMq61hq0Za6kwvj-kBA0C9WIOcYt37A', // Add your cookie here
          },
          credentials: 'include', // Include credentials (cookies) in the request
        });

        if (!response.ok) {
          throw new Error('Network response was not ok');
        }

        const data = await response.json();
        console.log(data); // Handle the response data as needed

      } catch (error) {
        console.error('Error fetching data:', error);
      }
    };

    fetchData();
  }, []);

  return (
    <div className="App">
      <header className="App-header">
        <img src={logo} className="App-logo" alt="logo" />
        <a href='https://localhost:7147/Identity/Account/Login'> Prova </a>
        <p>
          Edit <code>src/App.js</code> and save to reload.
        </p>
        <a
          className="App-link"
          href="https://reactjs.org"
          target="_blank"
          rel="noopener noreferrer"
        >
          Learn React
        </a>
      </header>
    </div>
  );
}

export default App;
