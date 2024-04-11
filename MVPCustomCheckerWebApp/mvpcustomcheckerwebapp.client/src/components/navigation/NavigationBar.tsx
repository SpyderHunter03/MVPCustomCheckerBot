import { useAuth0 } from '@auth0/auth0-react';
import { Link } from 'react-router-dom';

function NavigationBar() {
  const { isAuthenticated, user, loginWithRedirect, logout } = useAuth0();
  return (
    <nav className="w-full flex justify-between items-center p-4 bg-white shadow-md fixed top-0 z-50">
      <div>
        <Link to="/" className="mx-2 text-lg text-gray-700 hover:text-gray-900">Home</Link>
        <Link to="/available-molds" className="mx-2 text-lg text-gray-700 hover:text-gray-900">Available Molds</Link>
        {isAuthenticated && (
          <Link to="/previous-files" className="mx-2 text-lg text-gray-700 hover:text-gray-900">Previous Files</Link>
        )}
      </div>
      <div>
        {isAuthenticated ? (
          <div>
            <span className="text-lg mr-4">{user!.email || "Logged In"}</span>
            <button className="text-lg text-blue-500 hover:text-blue-700" onClick={() => logout({ returnTo: window.location.origin })}>
              Log Out
            </button>
          </div>
        ) : (
          <button className="text-lg text-blue-500 hover:text-blue-700" onClick={() => loginWithRedirect()}>Log In</button>
          /*<Link to = "/login" className = "text-lg text-blue-500 hover:text-blue-700">Login</Link>*/
        )}
      </div>
    </nav>
  );
}

export default NavigationBar;