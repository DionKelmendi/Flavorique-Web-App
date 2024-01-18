import { React, useState, useEffect } from 'react'
import { Link } from 'react-router-dom';
import CategoryItem from './CategoryItem';

export default function CategoryList({ amount, searchData }) {
  const [categoryData, setCategoryData] = useState([]);
  const [resultData, setResultData] = useState([]);
  const [error, setError] = useState([]);

  useEffect(() => {
    fetch('https://localhost:7147/api/Category/tags', {
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
        setCategoryData(data);
      })
      .catch(error => {
        setError(error.message);
        console.log(error);
      });

    if (searchData) {
      fetch(`https://localhost:7147/api/Tag?searchString=${searchData}`, {
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
          setResultData(data.data);
          console.log(data.data);
        })
        .catch(error => {
          setError(error.message);
          console.log(error);
        });
    }
  }, [searchData]);

  return (
    <>
      {searchData ?
        <>
          <section className="pt-4 pb-5" style={{ background: "#f7f7f7" }}>
            <div className='m-auto w-60'>
              <h4 className='m-0 mt-5 text-primary'><b> Results: </b></h4>
              <hr className='mt-2' />
              <ul className='coloredBullets  d-flex justify-content-between flex-wrap' style={{ letterSpacing: "1px", fontSize: "18px" }}>
                {resultData.length > 0 ?
                  resultData.map(tag => (
                    <li key={tag.id} className="p-2" style={{ width: "33%" }}><Link to={"/Tag/" + tag.id} className="text-secondary-emphasis text-decoration-none listHoverable">{tag.name}</Link></li>
                  ))
                  :
                  <li style={{ color: "black !important" }}>No tags found...</li>
                }
              </ul>
            </div>
          </section>
        </>
        :
        <>
          <section className="pt-4 pb-5" style={{ background: "#f7f7f7" }}>
            <div className='m-auto w-60'>
              {categoryData.map(data => (
                <CategoryItem key={data.category.name} category={data.category} tags={data.tags} />
              ))}
            </div>
          </section>
        </>
      }
    </>
  )
}