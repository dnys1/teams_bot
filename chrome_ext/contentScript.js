document.body.onload = () => {
    const url = document.getElementById('path').value;
    try {
        chrome.runtime.sendMessage({
            method: 'openLocalFile',
            localFileUrl: url
        });
    } catch (e) {
        console.log(`Error within extension: ${e.toString()}`);
    }
}