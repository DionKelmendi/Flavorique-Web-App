import './styles.css';
import { Link } from 'react-router-dom';
function Navbar(){
    return(
       <nav className="nav">
            <Link to ="/" className ="site-title">
                Flavorique
            </Link>
            <ul>
                <CustomLink to ="/">Home</CustomLink>
                <CustomLink to ="/Categories">Categories</CustomLink>
                <CustomLink to ="/Recipes">Recipes</CustomLink>
                </ul>

            </nav>
        
    )
}

function CustomLink({to , children, ...props}){
    const path = window.location.pathname
    return(
        <li className = {path === to ? "active" : ""}>
            <Link to ={to} {...props}>
                {children}
            </Link>
        </li>
    )
}

export default Navbar;
