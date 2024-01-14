import { React, useState, useEffect } from 'react'
import { Link } from 'react-router-dom';
import CategoryList from '../components/recipeComponents.js/CategoryList';
import CategoryItem from '../components/recipeComponents.js/CategoryItem';

export default function Categories({ }) {

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
            <div>
                <div className='m-auto text-center w-60 py-5'>
                    <h3 className='text-primary'><i className="bi bi-journal-text text-warning"></i> &nbsp; All Categories &nbsp; <i className="bi bi-journal-text text-warning"></i></h3>
                </div>
            </div>
            <CategoryList amount={5} />
            <div style={{ background: "#f7f7f7" }}>
                <div className='m-auto text-center w-60 pb-5'>
                    <p><b>MORE CATEGORIES</b></p>
                    <div className='paginationContainer'>
                        <a className="btn btn-primary me-2"> 1 </a>
                        <a className="btn btn-light me-2"> 2 </a>
                        <a className="btn btn-light me-2"> ... </a>
                        <a className="btn btn-light me-2"> 10 </a>
                        <a className="btn border"> <b>&gt;</b> </a>
                    </div>
                </div>
            </div>
        </>
    )
}