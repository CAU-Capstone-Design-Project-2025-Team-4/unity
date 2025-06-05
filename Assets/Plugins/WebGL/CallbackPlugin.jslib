mergeInto(LibraryManager.library, {
    ModelLoadCallback: function(idPtr) {
        var id = UTF8ToString(idPtr);
        
        if (typeof window.onModelLoad === "function") {
            window.onModelLoaded(id);
        }
    },
    
    CameraUpdateCallback: function(jsonPtr) {
        var jsonString = UTF8ToString(jsonPtr);
        
        if (typeof window.onCameraUpdate === "function") {
            window.onCameraUpdate(jsonString);
        }
    },
    
    AnimationListCallback: function(idPtr, animationListJsonPtr) {
        var modelId = UTF8ToString(idPtr);
        var animationListJson = UTF8ToString(animationListJsonPtr);
        
        if (typeof window.onAnimationList === "function") {
            window.onAnimationList(modelId, animationListJson);
        }
    },
    
    AnimationStateCallback: function(idPtr, stateJsonPtr) {
        var modelId = UTF8ToString(idPtr);
        var stateJson = UTF8ToString(stateJsonPtr);
        
        if (typeof window.onAnimationState === "function") {
            window.onAnimationState(modelId, stateJson);
        }
    },
    
    AnimationControlResultCallback: function(idPtr, resultPtr) {
        var modelId = UTF8ToString(idPtr);
        var result = UTF8ToString(resultPtr);
        
        if (typeof window.onAnimationControlResult === "function") {
            window.onAnimationControlResult(modelId, result);
        }
    }
});