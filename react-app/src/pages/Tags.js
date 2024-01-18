import { React, useState, useEffect } from 'react'
import { Link } from 'react-router-dom';
import CategoryList from '../components/recipeComponents.js/CategoryList';

export default function Tags({ }) {

  const [searchValue, setSearchValue] = useState('');
  const [error, setError] = useState([]);

  const handleInputChange = (e) => {
    setSearchValue(e.target.value);
  };

  return (
    <>
      <section className='tagContainer'>
        <section className='heroContainer' style={{ background: "white" }}>
          <div className="m-auto text-center p-5 pb-3 w-60">
            <h1 className="mb-5" style={{ fontSize: "50px" }}>Tags</h1>
            <h5 className='text-secondary w-75 m-auto text-justify'>Discover the essence of culinary variety with our extensive collection of tags. From savory to sweet, vegetarian delights to carnivorous feasts, whether you're on the hunt for easy recipes, indulgent treats, or healthy options, we've categorized them all.</h5>
            <form className='w-75 m-auto' style={{ transform: "translateY(2.5rem)" }}>
              <div className="form-group">
                <div className="input-group">
                  <span className="input-group-text">
                    <i className="bi bi-search"></i>
                  </span>
                  <input type="text" placeholder='Search by tag' className="form-control form-control-lg" value={searchValue} onChange={handleInputChange} />
                </div>
              </div>
            </form>
          </div>
        </section>
        <CategoryList amount={5} searchData={searchValue} />
        <div className="d-grid m-auto pb-5" style={{ background: "#f7f7f7" }}>
          <Link to="/Categories" className='btn btn-lg btn-primary w-25 m-auto'>
            <b>VIEW ALL CATEGORIES</b>
          </Link>
        </div>
      </section>
    </>
  )
}