mergeInto(LibraryManager.library, {
    // Send standard game state JSON (turn, current player, cards) to the browser window
    SendStateToReact: function(stateJson) {
        var jsonStr = UTF8ToString(stateJson);
        var event = new CustomEvent("UnityStateUpdated", { detail: JSON.parse(jsonStr) });
        window.dispatchEvent(event);
    },

    // Send quiz data JSON (question, options, metadata) to trigger the React Quiz Modal
    ShowQuizToReact: function(quizJson) {
        var jsonStr = UTF8ToString(quizJson);
        var event = new CustomEvent("UnityShowQuiz", { detail: JSON.parse(jsonStr) });
        window.dispatchEvent(event);
    },

    // Notify React that the game has finished and show the game over screen
    ShowGameOverToReact: function(gameOverJson) {
        var jsonStr = UTF8ToString(gameOverJson);
        var event = new CustomEvent("UnityShowGameOver", { detail: JSON.parse(jsonStr) });
        window.dispatchEvent(event);
    },

    // Notify React that the prologue narrative is starting
    ShowPrologueToReact: function(prologueJson) {
        var jsonStr = UTF8ToString(prologueJson);
        var event = new CustomEvent("UnityShowPrologue", { detail: JSON.parse(jsonStr) });
        window.dispatchEvent(event);
    }
});
