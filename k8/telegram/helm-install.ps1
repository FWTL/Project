param(
    [Parameter(Mandatory=$True)]
    [System.String]
    $AccountId,

    [Parameter(Mandatory=$True)]
    [System.String]
    $PathToChart
)

$installationName = "telegram-" + $AccountId
helm install $installationName $PathToChart --set accountId=$AccountId
