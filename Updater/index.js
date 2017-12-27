const fs = require('fs')
const path = require('path')
const firebase = require('firebase')
const googleStorage = require('@google-cloud/storage')

let pathToDownload = './'
const keypath = './nwgame-key.json'
const key = {
    "type": "service_account",
    "project_id": "nwgame-d8f9d",
    "private_key_id": "69a6a4b9abb8e87a7487d0aa0123c8a6553e2c9d",
    "private_key": "-----BEGIN PRIVATE KEY-----\nMIIEvAIBADANBgkqhkiG9w0BAQEFAASCBKYwggSiAgEAAoIBAQCeyJoU2wiR/WO+\nqtHa6ailYVbZnQehWfJJ+bLT30XRxD1pVI7hdFP5EiXZrg1g4PrR2WADMBQlrwpc\nWWEGIhneVzWzYuLV0s35etx5WXsyLFRGRyLKyn+kGAEjzcZjGuxiVasasHS0KM3q\nh2hHV2S2Pr7FabR4kfxJSnH7b3YuoVjUDoD+9nL8tJFq9rWAnVO++UMHMnxwhLK2\nlJUoU78FjBHnZoU6D7RPuBfY+FS0peQDJnjJw8KBYlf4ORk8qliwQVj5YqKYD1g0\n7bTVCNG2okAYhhPgDlR5MSeXhgu+GE3br+P9pDTDhP6fZLxBiGHaXlE29iUg2X84\n5q8PWcalAgMBAAECggEABavTCwcmt3YmK590xt2dGykOnc+kv9/TTk6Xk0cwenVI\nnzw4rTdHL4h1JyD3E9x9QOUDb6G2OoFVvKYpl7s8ksspHRrdULdKiYxYlxH52zkn\nmmn8hWM1q2H7GnEFUrFZu4plRdsDFBM4BZYoBJJLw9GaZTkpC7xlFyYMS6d3wGMO\n/kXZQVbe9ZM7B2H09EbxawyWuCLiCXuwnnsBlvtfaGTnFaOKyUvnskuk+UkGtiCY\n7GWy11N4mF9Dw2m/HzzsaQZQORr8BoLDxdnBsCahWNCVjICBSxirYyyGcJ5fsV3N\nv9eDsHSur1QbN/yF0UQWoNHuY5tyeQfaLlG73JbX4QKBgQDNfvKzoqzaOYqaK+zv\naP/zoSH4PHDNu7ERzxpBE4hjjOMNu+rV97XfHCfLgioue7au4UaGBk5pYRCl+W2m\n0vxaGeofN9ghxGiqCog+kSs4VtQBnzzsLhY3ZiHx4Kw8ZAnJrVAPrVhK+8SD7OG2\nQr6XdxMEVJLyhxmxBco/NeyrhQKBgQDFzq/arsN0BWRbMepuRcdZ1C6JkjrsfF5g\nVPeuqlt8von1X5weuLEpsUxMIMc0c1trmPY0p3snN68pYHxC+gnmpEKxOHtedXRl\nNJdwNJL75GuVOC3q9VDWy9EMvZ/kwumQqQ4MCgrhHJ2rDfh6MW9wHZ7/ZNM8LC7Y\nGx1KxBrIoQKBgAcRTKuc9rDyta+jvxxk0hjA+/gbrA2HXQmtw8tlLpWt7Mk2I1eE\nk36+6yd2rJi4rInLOa+2W9AIBJiQwZKxQvieV40HUKmopajjf9gXChN/+P6tSV0e\nIKl7G8PD5GiXUDbdfRls7tJ1YkbfSMwJ86Al+kN13S/2MY4DEvellSQ5AoGActnD\nOVHnomHPFzQwTb6uNPYB31bMZ6r+kShEd2HqpM9tAs90slHCa9bJ2hneEBI6roqt\nyXzFAMxQI9Il1C2hugz9vDBzUTEZV/nlQ/0CqsbBwG5SGu489bp5stAXGpghTSEb\nSto53uLimQMPlsNFVO+d9ePw0itFN6zmoRakBoECgYAVMOgwC7pNEWrXHt9gRkqD\nEuPBgoB05m7SOVWic+0bfpLUzHlIKXajdIT9VD5+Yrdxb1Hm6jA+RGuKAcTsyXKp\nl0d+IxZcUJcQ7P9Vsb37Td5uUlvde6enMXK/zo6BjFGUdWr7TL41KiPj9NBDL1Wc\n/fB3EnQYyMB0NTQgqVeRFA==\n-----END PRIVATE KEY-----\n",
    "client_email": "firebase-adminsdk-poqyv@nwgame-d8f9d.iam.gserviceaccount.com",
    "client_id": "113663855454809669461",
    "auth_uri": "https://accounts.google.com/o/oauth2/auth",
    "token_uri": "https://accounts.google.com/o/oauth2/token",
    "auth_provider_x509_cert_url": "https://www.googleapis.com/oauth2/v1/certs",
    "client_x509_cert_url": "https://www.googleapis.com/robot/v1/metadata/x509/firebase-adminsdk-poqyv%40nwgame-d8f9d.iam.gserviceaccount.com"
}
if(!fs.existsSync(keypath)) fs.appendFileSync(keypath, JSON.stringify(key))

const storage = googleStorage({
    projectId: 'nwgame-d8f9d',
    keyFilename: keypath
})

const bucket = storage.bucket('nwgame-d8f9d.appspot.com')
const bucketName = 'nwgame-d8f9d.appspot.com'

function downloadAllFilesFromDir(prefix) {
    const options = { prefix }

    storage
    .bucket(bucketName)
    .getFiles(options)
    .then(results => {
        const files = results[0]
        files.forEach(file => {
            const lastChar = file.name.substr(file.name.length - 1)
            if(lastChar === '/') return // if dir

            if(file.name === 'Windows/UnityPlayer.dll' && fs.existsSync(pathToDownload + 'UnityPlayer.dll')) return
            if(file.name === 'Windows/NW.exe' && fs.existsSync(pathToDownload + 'NW.exe')) return

            console.log(`Downloading ${file.name}`)
            downloadFile(file.name, pathToDownload + file.name.replace('Windows/', ''))
        })
    })
    .catch(err => {
        console.error('ERROR:', err)
    })
}

function downloadFile(srcFilename, destination) {
    const destPath = path.dirname(destination).split(path.sep)
    const pathToRemove = pathToDownload.split(path.sep)
    pathToRemove.pop()
    pathToRemove.forEach(x => destPath.shift())
    if(destPath.length > 0) {
        let pathToCreate = pathToDownload
        destPath.forEach(p => {
            pathToCreate += p + '/'
            if(!fs.existsSync(pathToCreate)) {
                fs.mkdirSync(pathToCreate)
            }
        })
    }

    const options = { destination }
    return storage
        .bucket(bucketName)
        .file(srcFilename)
        .download(options)
        .catch(err => {
            console.error('ERROR:', err, '\n\n File: ', srcFilename, '\n\n Dest:', destination)
        })
}

downloadAllFilesFromDir('Windows')
