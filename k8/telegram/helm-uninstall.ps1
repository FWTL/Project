param(
    [Parameter(Mandatory=$True)]
    [System.String]
    $AccountId,

    [Parameter(Mandatory=$True)]
    [System.String]
    $PathToChart
)

$installationName = "telegram-" + $AccountId
$pathToPatchFile = $PathToChart + "\\remove-path-from-ingress.json";

helm uninstall $installationName

$index = kubectl get ingress -o json | jq ('.items[0].spec.rules[0].http.paths | map(.path == "\"/Accounts/{{accountId}}/(.*)"\") | index(true)' | % {$_.replace("{{accountId}}",$AccountId)})
kubectl patch ingress telegram-ingress --type json --patch "$(Get-Content $pathToPatchFile -Raw | % {$_.replace("{{index}}",$index)})"