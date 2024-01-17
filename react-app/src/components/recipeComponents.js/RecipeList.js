import { React, useState, useEffect } from 'react'
import { Link } from 'react-router-dom';
import RecipeListItem from './RecipeListItem';

export default function RecipeList({ }) {

  const [recipeData, setRecipeData] = useState([])
  const [sortOrder, setSortOrder] = useState("")
  const [searchString, setSearchString] = useState("")
  const [pageNumber, setPageNumber] = useState(1)
  const [totalPages, setTotalPages] = useState(1)
  const [error, setError] = useState([])

  useEffect(() => {
    fetch(`https://localhost:7147/api/Recipe?sortOrder=${sortOrder}&searchString=${searchString}&pageNumber=${pageNumber}&pageSize=12`, {
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
        setRecipeData(data.data);
        setPageNumber(data.pageIndex);
        setTotalPages(data.totalPages);
      })
      .catch(error => {
        setError(error.message);
        console.log(error);
      });
  }, [sortOrder, searchString, pageNumber]);

  const recipeListItems = recipeData.map(recipe => (
    <RecipeListItem key={recipe.id} id={recipe.id} src={recipe.image} title={recipe.title} author={recipe.authorName} reviews="1658" rating="4.5" />
  ));

  const paginationButtons = Array(totalPages).fill(null).map((_, index) => (
    <a onClick={() => page(index + 1)} key={index} className={`btn mx-1 ${pageNumber === index + 1 ? 'btn-primary' : 'btn-light'}`}> {index + 1} </a>
  ));

  const handleSearchChange = (e) => {
    setSearchString(e.target.value);
  };
  const handleSortChange = (e) => {
    setSortOrder(e.target.value);

    console.log(e.target.value);
  };

  const page = (direction) => {
    if (direction == "down") {
      setPageNumber(pageNumber - 1);
    }
    else if (direction == "up") {
      setPageNumber(pageNumber + 1);
    }
    else {
      setPageNumber(direction);
    }
  }

  return (
    <>
      <section>
        <div className="mt-5 " style={{ background: "#f7f7f7" }}>
          <div className='m-auto text-center w-60 py-5'>
            <h3 className='text-primary mb-5'><i className="bi bi-journal-text text-warning"></i> &nbsp; All Recipes &nbsp; <i className="bi bi-journal-text text-warning"></i></h3>
            <div className='w-75 my-3' style={{ textAlign: "left" }}>
            </div>
            <div className='d-flex' style={{ gap: "10px" }}>
              <div className="input-group">
                <span className="input-group-text">
                  <i className="bi bi-search"></i>
                </span>
                <input id="search" name='search' type="text" placeholder='Search using Title or Author' className="form-control rounded" value={searchString} onChange={handleSearchChange} />
              </div>
              <select className="form-select w-25" onChange={handleSortChange} defaultValue="">
                <option value="">Sort By</option>
                <option value="title">Title</option>
                <option value="titleDesc">Title Descending</option>
                <option value="date">Oldest Recipes</option>
                <option value="dateDesc">Most Recent Recipes</option>
              </select>
            </div>
          </div>
        </div>
        <div className='m-auto text-center w-60 py-5'>

          <div className='d-flex flex-wrap'>
            {recipeListItems}
          </div>
          <p className='mt-5'><b>MORE RECIPES</b></p>
          <div className='paginationContainer'>

            <a className={`btn border ${pageNumber == 1 ? "d-none" : ""}`} onClick={() => page('down')}> <b>&lt;</b> </a>

            {paginationButtons}

            <a className={`btn border ${pageNumber == totalPages ? "d-none" : ""}`} onClick={() => page('up')}> <b>&gt;</b> </a>
          </div>
        </div>
      </section>
    </>
  )
}