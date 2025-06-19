chrome.contextMenus.create({
  id: "youtubeMenuItem",
  title: "Download Video",
  contexts: ["link", "page"],
  documentUrlPatterns: ["*://www.youtube.com/*"],
});
chrome.contextMenus.create({
  id: "youtubeMenuItem-cookies",
  title: "⚠️Download Video (Cookies)",
  contexts: ["link", "page"],
  documentUrlPatterns: ["*://www.youtube.com/*"],
});

// Handle the click event for the context menu
chrome.contextMenus.onClicked.addListener((info, tab) => {
  if (info.menuItemId === "youtubeMenuItem") {
    let url = info.linkUrl ? info.linkUrl : tab.url;

    if (url.includes("/shorts/")) url = url.replace("shorts/", "watch?v=");

    if (!url.includes("watch?v=")) return;

    sendDataToHost(url, false);
  } else if (info.menuItemId === "youtubeMenuItem-cookies") {
    let url = info.linkUrl ? info.linkUrl : tab.url;

    if (url.includes("/shorts/")) url = url.replace("shorts/", "watch?v=");

    if (!url.includes("watch?v=")) return;
    sendDataToHost(url, tab, true);
  }
});

function sendDataToHost(url, tab, withCookies) {
  if (withCookies) {
    // send with cookies
    console.log("Sending with cookies.😀");
    //return;
    const tabUrl = new URL(tab.url);
    console.log(tab);
    browser.cookies
      .getAll({
        url: tabUrl.href,
        partitionKey: { topLevelSite: url.origin },
        storeId: tab.cookieStoreId,
      })
      .then((cookies) => {
        //console.log(cookies);
        let cookieFileContent = "# Netscape HTTP Cookie File\n";
        cookies.forEach((cookie) => {
          // Format each cookie according to the Netscape HTTP Cookie File format
          const line = [
            cookie.domain,
            cookie.hostOnly ? "FALSE" : "TRUE",
            cookie.path,
            cookie.httpOnly ? "FALSE" : "TRUE",
            cookie.expirationDate,
            cookie.name,
            cookie.value,
          ].join("\t");

          cookieFileContent += line + "\n";
        });
        //console.log(cookieFileContent)
        const sending = browser.runtime.sendNativeMessage(
          "sytd_host",
          url + "," + cookieFileContent
        );
        sending.then(onResponse, onError);
      });
  } else {
    console.log("Sending without cookies.☹️");
    //return;
    let sending = browser.runtime.sendNativeMessage("sytd_host", url);
    sending.then(onResponse, onError);
  }
}

function onResponse(response) {
  console.log(`Received ${response}`);
}

function onError(error) {
  console.log(`Error: ${error}`);
}

function getStoreId() {
  const tab = browser.tabs.query({ active: true, currentWindow: true });
  if (tab.cookieStoreId) return tab.cookieStoreId;
}
