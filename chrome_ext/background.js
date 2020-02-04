chrome.runtime.onMessage.addListener((message, sender) => {
    if (message.method == 'openLocalFile') {
        // Send the url to our native host if it's a file link
        const localFileUrl = message.localFileUrl;
        if (localFileUrl.indexOf('file:') !== -1) {
            chrome.runtime.sendNativeMessage('com.brwncald.open_folder',
                { path: localFileUrl }
            );

            // Close the tab
            const tab = sender.tab;
            chrome.tabs.remove(tab.id);
        }
    }
});