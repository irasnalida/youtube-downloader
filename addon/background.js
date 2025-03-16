chrome.contextMenus.create({
  id: "youtubeMenuItem",
  title: "Download Video",
  contexts: ["link", "page"],
  documentUrlPatterns: ["*://www.youtube.com/*"]
});

// Handle the click event for the context menu
chrome.contextMenus.onClicked.addListener((info, tab) => {
  if (info.menuItemId === "youtubeMenuItem") {
    let url = (info.linkUrl)? info.linkUrl : tab.url;

    if (url.includes("/shorts/")) url = url.replace("shorts/", "watch?v=");

    if (!url.includes("watch?v=")) return;

    // Check if the clicked item is a link
    let sending = browser.runtime.sendNativeMessage("sytd_host", url);
    sending.then(onResponse, onError);
    console.log(url);
  }
});

function onResponse(response) {
  console.log(`Received ${response}`);
}

function onError(error) {
  console.log(`Error: ${error}`);
}