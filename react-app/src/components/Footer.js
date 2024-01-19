import { React, useState, useEffect } from 'react'
import { Link } from 'react-router-dom';

export default function Footer({ userData }) {

  const [nameValue, setNameValue] = useState('');
  const [emailValue, setEmailValue] = useState('');
  const [tagData, setTagData] = useState([]);
  const [error, setError] = useState([]);

  useEffect(() => {
    fetch('https://localhost:7147/api/Tag?pageSize=7', {
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
        setTagData(data.data);
      })
      .catch(error => {
        setError(error.message);
        console.log(error);
      });
  }, []);

  const handleNameChange = (e) => {
    setNameValue(e.target.value);
  };
  const handleEmailChange = (e) => {
    setEmailValue(e.target.value);
  };
  const autoChangeValues = () => {
    setNameValue(userData.userName);
    setEmailValue(userData.email);
  };
  const handleFormSubmit = (e) => {
    e.preventDefault();
    console.log('Name value:', nameValue);
    console.log('Email value:', emailValue);
  };

  const tags = tagData.map(tag => (
    <li key={tag.id}>
      <Link to={"/Tag/" + tag.id} className='text-secondary-emphasis text-decoration-none' style={{ letterSpacing: "1px" }}>{tag.name}</Link>
    </li>
  ));

  return (
    <footer className='p-5 pb-4' style={{ background: "white" }}>
      <div className='m-auto d-flex justify-content-between' style={{ width: "calc(60% + 64px)", color: "black" }}>
        <div>
          <h5 style={{ paddingLeft: "32px" }}>Flavorique</h5>
          <ul style={{ listStyleType: "none" }}>
            <li>
              <Link to="/" className='text-secondary-emphasis text-decoration-none' style={{ letterSpacing: "1px" }}>Home</Link>
            </li>
            <li>
              <Link to="/About" className='text-secondary-emphasis text-decoration-none' style={{ letterSpacing: "1px" }}>About Us</Link>
            </li>
            <li>
              <Link to="/Recipes/All" className='text-secondary-emphasis text-decoration-none' style={{ letterSpacing: "1px" }}>Recipes</Link>
            </li>
            <li>
              <Link to="/Tags" className='text-secondary-emphasis text-decoration-none' style={{ letterSpacing: "1px" }}>Tags</Link>
            </li>
            <li>
              <Link to="/Recipes" className='text-secondary-emphasis text-decoration-none' style={{ letterSpacing: "1px" }}>Our Top Recipes</Link>
            </li>
          </ul>
        </div>
        <div>
          <h5 style={{ paddingLeft: "32px" }}>Food & Recipes</h5>
          <ul style={{ listStyleType: "none" }}>
            {tags}
          </ul>
        </div>
        <div className='bg-primary p-4 text-center d-flex flex-column justify-content-center align-items-center' style={{ color: "white", height: "200px", gap: "20px" }}>
          <div>
            <h5 className='m-0'>Sign up for email updates</h5>
            <p className='m-0'>Get notified when a new change is added</p>
          </div>
          <form id="footerForm" onSubmit={handleFormSubmit}>
            <div className="input-group">
              <input type="text" placeholder='Name' className="form-control rounded me-2" value={nameValue} onChange={handleNameChange} />
              <input type="text" placeholder='Email' className="form-control rounded me-2" value={emailValue} onChange={handleEmailChange} />
              <button type="submit" className='btn btn-lg btn-warning rounded'><b>GO</b></button>
            </div>
          </form>
          {userData ? (
            <>
              <p className='m-0'>Or sign up using your <span style={{ cursor: "pointer" }} onClick={autoChangeValues} className='text-warning'>account</span></p>
            </>
          ) : (
            <>
              <p className='m-0'>Or sign up using your <a href="https://localhost:7147/Identity/Account/Login/" className='text-warning'>account</a></p>
            </>
          )}
        </div>
      </div>
      <div className="m-auto d-flex justify-content-between w-60">
        <div className="d-flex align-items-center">
          <img height={75} src="https://localhost:7147/logo" style={{ filter: "grayscale(30%)" }} />
          <h2 className='m-0 ms-3 me-4'>Flavorique</h2>
          <p className="m-0" style={{ fontSize: "12px", width: "115px" }}>
            © 2024 Flavorique. All Rights Reserved.
          </p>
        </div>
        <div className='d-flex justify-content-center align-items-center w-50'>
          <Link to="/contact" className='rounded bg-primary text-center me-3 hoverable' style={{ color: "white", width: "40px", fontSize: "27px" }}>
            <i className="bi bi-instagram"></i>
          </Link>
          <Link to="/contact" className='rounded bg-primary text-center me-3 hoverable' style={{ color: "white", width: "40px", fontSize: "27px" }}>
            <i className="bi bi-pinterest"></i>
          </Link>
          <Link to="/contact" className='rounded bg-primary text-center me-3 hoverable' style={{ color: "white", width: "40px", fontSize: "27px" }}>
            <i className="bi bi-facebook"></i>
          </Link>
          <Link to="/contact" className='rounded bg-primary text-center me-3 hoverable' style={{ color: "white", width: "40px", fontSize: "27px" }}>
            <i className="bi bi-twitter"></i>
          </Link>
        </div>
      </div>
      <div className="m-auto mt-4 d-flex justify-content-center w-60" style={{ gap: "20px" }}>
        <Link to="/Privacy" className='text-secondary-emphasis text-decoration-none' style={{ fontSize: "14px" }}>Privacy Policy</Link>
        <Link to="/Terms" className='text-secondary-emphasis text-decoration-none' style={{ fontSize: "14px" }}>Terms of Service</Link>
      </div>
    </footer>
  )
}