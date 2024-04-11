import NavigationBar from "../components/navigation/NavigationBar";

function LandingPage() {
    return (
      <div className="flex flex-col min-h-screen">
        <NavigationBar />
        {/* Adding padding-top to ensure content below the navbar is not obscured */}
        <div className="pt-16 text-center">
          <span className="text-xl uppercase font-bold">MVP Custom File Checker</span>
          {/* Add more content here */}
        </div>
      </div>
    );
}

export default LandingPage;