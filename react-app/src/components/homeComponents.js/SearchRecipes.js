import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';

export default function SearchRecipes() {
  const [searchValue, setSearchValue] = useState('');
  const navigate = useNavigate();

  const handleInputChange = (e) => {
    setSearchValue(e.target.value);
  };

  const handleFormSubmit = (e) => {
    e.preventDefault();
    navigate('/Recipes/All', { state: { data: searchValue } })
  };
  return (
    <>
      <form onSubmit={handleFormSubmit}>
        <div className="form-group">
          <div className="input-group">
            <Link className='input-group-text' to="/Recipes/All" state={{ data: searchValue }}>
              <i className="bi bi-search"></i>
            </Link>
            <input type="text" placeholder='Search our recipes' className="form-control form-control-lg" value={searchValue} onChange={handleInputChange} />
          </div>
        </div>
      </form>
      <h5 className='m-0'>
        or
      </h5>
      <Link to="/Recipes/All" className='btn btn-lg btn-primary'>
        View All Recipes
      </Link>
    </>
  );
}
