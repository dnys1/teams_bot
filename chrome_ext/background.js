chrome.runtime.onMessage.addListener((message, sender) => {
    if (message.method == 'openLocalFile') {
        // Send the url to our native host
        const localFileUrl = message.localFileUrl;
        chrome.runtime.sendNativeMessage('com.brwncald.open_folder',
            { path: localFileUrl }
        );

        // Close the tab
        const tab = sender.tab;
        chrome.tabs.remove(tab.id);
    }
});