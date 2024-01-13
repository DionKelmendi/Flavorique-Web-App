import React, { useState } from 'react';
import { Link } from 'react-router-dom';

export default function SearchRecipes() {
  const [searchValue, setSearchValue] = useState('');

  const handleInputChange = (e) => {
    setSearchValue(e.target.value);
  };

  const handleFormSubmit = (e) => {
    e.preventDefault();
    console.log('Search value:', searchValue);
  };

  return (
    <>
      <form onSubmit={handleFormSubmit}>
        <div className="form-group">
          <div className="input-group">
            <span className="input-group-text">
              <i className="bi bi-search"></i>
            </span>
            <input type="text" placeholder='Search our recipes' className="form-control form-control-lg" value={searchValue} onChange={handleInputChange} />
          </div>
        </div>
      </form>
      <h5 className='m-0'>
        or
      </h5>
      <Link to="/recipes" className='btn btn-lg btn-primary'>
        View All Recipes
      </Link>
    </>
  );
}