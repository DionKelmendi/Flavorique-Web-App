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
    "https://ckbox.cloud/nCX3ISMpdWvIZzPqyw4h/assets/mv6qWubeoYd6/images/680.webp",
    "https://ckbox.cloud/nCX3ISMpdWvIZzPqyw4h/assets/krvPN00Fvp9H/images/680.webp",
    "https://ckbox.cloud/nCX3ISMpdWvIZzPqyw4h/assets/zByxZ5IE7wxG/images/680.webp",
    "https://ckbox.cloud/nCX3ISMpdWvIZzPqyw4h/assets/liq9puvy8Q5z/images/680.webp",
    "https://ckbox.cloud/nCX3ISMpdWvIZzPqyw4h/assets/ZDUD-NTfcj8A/images/680.webp",
    "https://ckbox.cloud/nCX3ISMpdWvIZzPqyw4h/assets/TxFl2a69d4LN/images/1200.webp",
    "https://ckbox.cloud/nCX3ISMpdWvIZzPqyw4h/assets/m-STdm_Joqzk/images/680.webp",
    "https://ckbox.cloud/nCX3ISMpdWvIZzPqyw4h/assets/ISgD0IP7WTSI/images/680.webp",
    "https://ckbox.cloud/nCX3ISMpdWvIZzPqyw4h/assets/M9ihXcrtknTl/images/680.webp",
    "https://ckbox.cloud/nCX3ISMpdWvIZzPqyw4h/assets/p79KVMLl_PZb/images/680.webp",
    "https://ckbox.cloud/nCX3ISMpdWvIZzPqyw4h/assets/GnIckvxAB6L1/images/680.webp",
    "https://ckbox.cloud/nCX3ISMpdWvIZzPqyw4h/assets/BFd36bA5aAf9/images/680.webp",
    "https://ckbox.cloud/nCX3ISMpdWvIZzPqyw4h/assets/DcxqGfRU5fDZ/images/680.webp"
  ];

  const bigTagItems = tagData.slice(0, 4).map((tag, index) => (
    <BigTagItem key={tag.id} id={tag.id} name={tag.name.toUpperCase()} src={imageArray[index]} index={index} />
  ));

  const smallTagItems = tagData.slice(4).map((tag, index) => (
    <SmallTagItem key={tag.id} id={tag.id} name={tag.name.toUpperCase()} src={imageArray[index + 4]} />
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