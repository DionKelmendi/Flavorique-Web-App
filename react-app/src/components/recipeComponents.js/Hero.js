import { React, useState, useEffect } from 'react'
import { Link } from 'react-router-dom';

export default function Hero({ userData }) {

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
      <section className='heroContainer' style={{ background: "#f7f7f7" }}>
        <div className="m-auto text-center p-5 pb-3 w-60">
          <h1 className="mb-5" style={{ fontSize: "50px" }}>Recipes</h1>
          <h5 className='text-secondary w-75 m-auto text-justify'>We’ve organized these recipes every way we could think of so you don't have to! Dietary restrictions, weeknight dinners, meal prep recipes, some of our most tried-and-true… no matter how you browse, we’re sure you’ll find just what you were looking for.</h5>
          <div className='mt-5'> {userData ? (
            <>
              <Link to="Create" className='text-decoration-none text-secondary-emphasis'>
                <button className='btn btn-lg btn-primary'><i className="bi bi-plus"></i> Create your own Recipe <i className="bi bi-plus"></i></button>
              </Link>
            </>
          ) : (
            <>
            </>
          )}</div>
          <form onSubmit={handleFormSubmit} className='w-75 m-auto' style={{ transform: "translateY(2.5rem)" }}>
            <div className="form-group">
              <div className="input-group">
                <span className="input-group-text">
                  <i className="bi bi-search"></i>
                </span>
                <input type="text" placeholder='Search our recipes' className="form-control form-control-lg" value={searchValue} onChange={handleInputChange} />
              </div>
            </div>
          </form>
        </div>
      </section>
    </>
  )
}
