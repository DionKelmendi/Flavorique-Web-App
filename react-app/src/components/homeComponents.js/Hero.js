import { React, useState, useEffect } from 'react'
import BigTagItem from './BigTagItem'
import SmallTagItem from './SmallTagItem'
import SearchRecipes from './SearchRecipes'
import { Link } from 'react-router-dom';

export default function Hero({ }) {

  const [tagData, setTagData] = useState([]);
  const [error, setError] = useState([]);

  useEffect(() => {
    fetch('https://localhost:7147/api/Tag?pageSize=13', {
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

  let imageArray = [
    "",
    "",
    "",
    "",
    "",
    "",
    "",
    "",
    ""
  ];

  const bigTagItems = tagData.slice(0, 4).map(tag => (
    <BigTagItem key={tag.id} id={tag.id} name={tag.name.toUpperCase()} src="https://pinchofyum.com/wp-content/uploads/cropped-Tofu-and-Brown-Rice-Lettuce-Wraps-Square.png" />
  ));

  const smallTagItems = tagData.slice(4).map(tag => (
    <SmallTagItem key={tag.id} id={tag.id} name={tag.name.toUpperCase()} src="https://pinchofyum.com/wp-content/uploads/cropped-Tofu-and-Brown-Rice-Lettuce-Wraps-Square.png" />
  ));

  return (
    <>
      <section className='heroContainer' style={{ background: "#f7f7f7" }}>
        <div className='m-auto pb-5 w-60'>
          <div className='d-flex w-100 justify-content-center p-5' style={{ gap: "15px" }}>
            {bigTagItems}
          </div>
          <div className='d-flex w-100 justify-content-center' style={{ gap: "20px" }}>
            {smallTagItems}
          </div>
          <div className='d-flex w-100 justify-content-center align-items-center mt-3' style={{ gap: "20px" }}>
            <SearchRecipes />
          </div>
        </div>
      </section>
    </>
  )
}