function initializeInactivityTimer(dotnetHelper) {
    var timer;
    //the timer will be reset whenever the user clicks the mouse or presses the keyboard
    document.onmousemove = resetTimer;
    document.onkeypress = resetTimer;

    function resetTimer() {
        clearTimeout(timer);
        //timer = setTimeout(logout, 6000); //600,000 milliseconds = 10 minuts
        timer = setTimeout(logout, 1800000); //600,000 milliseconds = 10 minuts
    }

    function logout() {
        dotnetHelper.invokeMethodAsync("Logout");
    }

}